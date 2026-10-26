using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Batterie_Sim
{
    public class Batterie

    {

        private readonly BatterieConfig _config;
        //taper, langsamer aufladen bei mehr prozent
        private const double TaperStartPercent = 80.0;
        private const double MinTaperFactor = 0.1;
        private const double DeadbandKW = 0.01;

        //Bremsen wenn batterie heiß wird
        public const double DefaultAmbientC = 20.0;
        private const double HeatFactor = 0.06;
        private const double CoolFactor = 0.02;
        private const double DerateStartC = 45.0;
        private const double DerateEndC = 60.0;
        private const double FaultHeatPerMin = 2;
        private bool tripped;
        private bool faultHeatActive;
        
        private bool isOn=true;

        public string DeviceId => _config.DeviceId;
        public BatterieConfig Config => _config;
        public bool IsOn => isOn;
        public double SocPercent { get; private set; }
        public double SetpointKW { get; private set; }
        public double ActualKW { get; private set; }
        public double TemperaturC { get; private set; }

        /// <summary>Aussentemperatur in °C (Standard 20; kommt mit Wetterbezug vom Wettersimulator).</summary>
        public double AmbientTemperatureC { get; set; } = DefaultAmbientC;
        public DeviceState State { get; private set; }
        public FaultType ActiveFault { get; private set; }

        public double ChargedKWh {  get; private set; }
        public double DischargeKWh { get; private set; }

        public double EquivalentCycles => (ChargedKWh + DischargeKWh) / (2 * _config.CapacityKWh);

        public Batterie(BatterieConfig config)
        {
            _config = config;
            SocPercent = config.StartSocPercent;
            State = DeviceState.Standby;
            TemperaturC = AmbientTemperatureC;
            ActiveFault = FaultType.None;
        }


        public void SetPower(double kW)
        {
            SetpointKW = kW;
        }
        public void Update(TimeSpan dt) => Update(dt.TotalSeconds);

        public void Update(double dtSeconds)
        {
            double dthours = dtSeconds / 3600;
            double dtminutes = dtSeconds / 60;
            ActualKW = CalculatePower(dthours);

            if (ActualKW>0)
            {
                ChargedKWh += ActualKW * dthours;
            }
            else if(ActualKW<0)
            {
                DischargeKWh += Math.Abs(ActualKW) * dthours;
            }

            SocPercent += ActualKW * dthours / _config.CapacityKWh * 100;
            UpdateTemperature(dtminutes);
            if (TemperaturC >= DerateEndC&& !tripped)
            {
                tripped = true;
                ActiveFault = FaultType.Overheat;
            }
            if (tripped && ActiveFault == FaultType.None && TemperaturC <= 50)
            {
                tripped = false;
            }
            UpdateState();
            
        }

        private double CalculatePower(double dthours)
        {
            if (ActiveFault == FaultType.EmergencyStop|| tripped || !isOn)
            {
                return 0;
            }
            double derate = GetDerateFactor();
            double Power = Math.Clamp(SetpointKW, -_config.MaxDischargeKW * derate, _config.MaxChargeKW * GetTaperFactor() * derate);

            if (Power > 0 && SocPercent >= _config.SocMaxPercent)
            { Power = 0; }
            else if (Power < 0 && SocPercent <= _config.SocMinPercent)
            { Power = 0; }
            else if (Power < 0)
            {
                double StoredKWh = (_config.SocMinPercent - SocPercent) / 100 * _config.CapacityKWh;
                double restKw = StoredKWh / dthours;
                Power = Math.Max(Power, restKw);
            }
            else if (Power > 0)
            {
                double Chargeroom = (_config.SocMaxPercent - SocPercent) / 100 * _config.CapacityKWh;
                double restKw = Chargeroom / dthours;
                Power = Math.Min(Power, restKw);
            }
            return Power;

        }

        private void UpdateState()
        {
            if ( !isOn)
            {
                State = DeviceState.Off;
            }
            else    if (tripped && ActiveFault==FaultType.Overheat)
            {
                State = DeviceState.Fault;
            }
            else if (tripped &&ActiveFault!=FaultType.Overheat)
            {  
                State = DeviceState.Cooldown; 
            }
            else if (ActiveFault == FaultType.EmergencyStop)
            {
                State = DeviceState.Fault;
            }
            else if (ActualKW < -DeadbandKW)
            {
                State = DeviceState.Discharging;
            }
            else if (ActualKW > DeadbandKW)
            {
                State = DeviceState.Charging;
            }
            else
            {
                State = DeviceState.Standby;
            }
        }

        private double GetTaperFactor()
        {
            if (SocPercent <= TaperStartPercent || _config.SocMaxPercent <= TaperStartPercent) return 1.0;
            // 1) Wie weit ist der SOC durch den Taper-Bereich?  (0 = Start, 1 = SocMax)
            double progress = (SocPercent - TaperStartPercent) / (_config.SocMaxPercent - TaperStartPercent);

            // 2) Faktor fällt linear von 1.0 auf MinTaperFactor
            double factor = 1.0 - (1.0 - MinTaperFactor) * progress;
            ;
            return Math.Max(factor, MinTaperFactor);
        }

        private void UpdateTemperature(double dtminutes)
        {
            double faultheat = 0;
            if (faultHeatActive)
            {
                faultheat = FaultHeatPerMin;
            }
            double heating = HeatFactor * Math.Abs(ActualKW);
            double cooling = CoolFactor * (TemperaturC - AmbientTemperatureC);
            TemperaturC += (heating+faultheat - cooling) * dtminutes;
        }

        private double GetDerateFactor()
        {
            if (TemperaturC <= DerateStartC) return 1.0;
            double progress = (TemperaturC - DerateStartC) / (DerateEndC - DerateStartC);
            double factor = 1.0 - progress;
            return Math.Clamp(factor, 0, 1);
        }



        public void TriggerFault(FaultType fault)
        {
            ActiveFault = fault;

            if (fault == FaultType.EmergencyStop)
            {
                SetpointKW = 0;
            }
            if(fault==FaultType.Overheat)
            {
                faultHeatActive = true; 
            }
        }
        public void ResetFault()
        {
            ActiveFault=FaultType.None;
            faultHeatActive = false;

        }

        public void TurnOff()
        { 
            isOn = false;
            SetpointKW = 0;
        }
        public void TurnOn()
        {
            isOn = true;
        }

    }
}
