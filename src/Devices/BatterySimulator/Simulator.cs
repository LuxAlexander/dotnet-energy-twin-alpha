using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Batterie_Sim
{
    public class Simulator
    {
        private readonly Batterie _battery;
        private readonly DailyProfile _profile;
        private readonly CsvLogger _logger;
        private readonly ConsoleDashboard _dashboard;
        private double _accumulatedSimSeconds;



        public double Speed { get; set; } = 600;
        public TimeSpan SimTime { get; private set; }
        public double StepSeconds { get; set; } = 60;
        public ControlMode Mode { get; set; } = ControlMode.Auto;

        public bool IsPaused { get; private set; }
        public bool QuitRequested { get; private set; }

            

        public Simulator(Batterie battery, DailyProfile profile, TimeSpan startTime, CsvLogger logger, ConsoleDashboard dashboard)
        {
            _battery = battery;
            _profile = profile;
            SimTime = startTime;
            _logger = logger;
            _dashboard = dashboard;
        }

        public void Step()
        {
            if (Mode == ControlMode.Auto)
            {
                TimeSpan time = new TimeSpan(SimTime.Hours, SimTime.Minutes, SimTime.Seconds);
                _battery.SetPower(_profile.GetSetpoint(time));
            }
            _battery.Update(StepSeconds);

            SimTime += TimeSpan.FromSeconds(StepSeconds);
            _logger.Log(SimTime, _battery, Mode);
        }

        public void Run(double durationHours)
        {
            TimeSpan endTime = SimTime + TimeSpan.FromHours(durationHours);
            var stopwatch = Stopwatch.StartNew();
            double lastRealSeconds = 0;

            while (!QuitRequested&& (durationHours == 0 || SimTime < endTime))
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKey key = Console.ReadKey(true).Key;   // true = Taste nicht anzeigen
                    HandleKey(key);
                }
                // 1) Wie viel echte Zeit ist seit dem letzten Durchlauf vergangen?
                if (IsPaused)
                {
                    ShowPauseMenu();
                    IsPaused = false;
                    lastRealSeconds = stopwatch.Elapsed.TotalSeconds;
                }
                double now=stopwatch.Elapsed.TotalSeconds;
                double deltatime = now - lastRealSeconds;
                lastRealSeconds = now;

                // 2) Das Konto um (Realzeit × Speed) erhöhen
                if (!IsPaused)
                {
                    _accumulatedSimSeconds += deltatime * Speed;
                }
                // 3) Solange das Konto mindestens einen Schritt enthält: Step() ausführen und StepSeconds abbuchen
                while (_accumulatedSimSeconds>=StepSeconds)
                { 
                    Step();
                    _accumulatedSimSeconds -= StepSeconds;
                }

                // 4) Eine Statuszeile ausgeben
                /* if(IsPaused)
                 {
                     Console.WriteLine("Pause");
                 }
                 Console.WriteLine($"{SimTime:hh\\:mm}  Soll {_battery.SetpointKW:F2}  Ist {_battery.ActualKW:F2}  SOC {_battery.SocPercent:F1} %  {_battery.State}  {_battery.TemperaturC:F1} °C");
                */
                _dashboard.Render(this, _battery);
                Thread.Sleep(100);
            }
            _logger.Close();

        }

        private void HandleKey(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.Spacebar:
                    IsPaused = !IsPaused;
                    if (IsPaused)
                    {
                        _logger.Flush();
                    }
                    break;
                case ConsoleKey.M:
                    if(Mode==ControlMode.Auto)
                    {
                        Mode = ControlMode.Manual;
                        _battery.SetPower(0);
                    }
                    else if(Mode==ControlMode.Manual)
                    {
                        Mode=ControlMode.Auto;
                    }
                    break;
                case ConsoleKey.Q:
                    QuitRequested = true;
                    break;
                case ConsoleKey.UpArrow:
                    if(Mode==ControlMode.Manual)
                    { _battery.SetPower(_battery.SetpointKW + 0.5); }
                    break;
                case ConsoleKey.DownArrow:
                    if (Mode == ControlMode.Manual)
                    { _battery.SetPower(_battery.SetpointKW - 0.5); }
                    break;
                case ConsoleKey.D0:
                    _battery.SetPower(0);
                    break;
                case ConsoleKey.NumPad0:
                    _battery.SetPower(0);
                    break;
                case ConsoleKey.E:
                    if(_battery.IsOn==true)
                    {
                        _battery.TurnOff();
                    }
                    else if (_battery.IsOn == false)
                    {
                        _battery.TurnOn();
                    }
                    break;
                case ConsoleKey.H:
                    _battery.TriggerFault(FaultType.Overheat);
                    break;
                case ConsoleKey.N:
                    _battery.TriggerFault(FaultType.EmergencyStop);
                    break;
                case ConsoleKey.R:
                    _battery.ResetFault();
                    break;
            }

        }
        private void ShowPauseMenu()
        {
            string message = "";
            Console.CursorVisible = true;
            bool resume = false;

            while (!resume)
            {
                Console.Clear();
                Console.WriteLine("=== PAUSE ===");
                Console.WriteLine(message);
                message = "";
                // aktuelle Werte anzeigen (siehe unten)
                // die 8 Menüpunkte anzeigen
                Console.WriteLine($"1:Sollwert {_battery.SetpointKW}\n2:Ladeleistung {_battery.Config.MaxChargeKW}\n3:Entladeleistung {_battery.Config.MaxDischargeKW}\n4:SOC-Min {_battery.Config.SocMinPercent}\n5:SOC-Max {_battery.Config.SocMaxPercent}\n6:Speed {this.Speed}\n7:Step {this.StepSeconds}\n8:Fortsetzen");

                string input = Console.ReadLine() ?? "";
                switch (input)
                {
                    case "1":
                        double? Setpoint= ReadDouble("Neuer Sollwert");
                        if (Setpoint !=null)
                        {
                            _battery.SetPower(Setpoint.Value);
                            Mode = ControlMode.Manual;
                        }
                        else
                        { message=("ungültige Eingabe"); }
                        break;
                    case "2": /* max. Ladeleistung */
                        double? charge = ReadDouble("Neue max. Ladeleistung in kW: ");
                        if (charge != null && charge > 0)
                        {
                            _battery.Config.MaxChargeKW = charge.Value;
                        }
                        else
                        {
                            message = "Ungültige Eingabe, Wert unverändert.";
                        }
                        break;
                    case "3":
                        double? discharge = ReadDouble("Neue max. Entladeleistung in kW: ");
                        if (discharge != null && discharge > 0)
                        {
                            _battery.Config.MaxDischargeKW = discharge.Value;
                        }
                        else
                        {
                            message = "Ungültige Eingabe, Wert unverändert.";
                        }
                        break;
                    case "4":
                        double? minsol = ReadDouble("Neue SOC-Min in Prozent: ");
                        if (minsol > 0 && minsol < _battery.Config.SocMaxPercent)
                        {
                            _battery.Config.SocMinPercent = minsol.Value;
                        }
                        else
                        {
                            message = "Ungültige Eingabe, Wert unverändert.";
                        }
                        break;
                    case "5":
                        double? maxsol = ReadDouble("Neue SOC-Max in Prozent: ");
                        if (maxsol < 100 && maxsol > _battery.Config.SocMinPercent)
                        {
                            _battery.Config.SocMaxPercent = maxsol.Value;
                        }
                        else
                        {
                            message = "Ungültige Eingabe, Wert unverändert.";
                        }
                        break;
                    case "6":
                        double? speed = ReadDouble("Neuer Speed: ");
                        if (speed != null && speed > 0)
                        {
                            Speed = speed.Value;
                        }
                        else
                        {
                            message = "Ungültige Eingabe, Wert unverändert.";
                        }
                        break;
                    case "7":
                        double? stepssize = ReadDouble("Neue Stepsize : ");
                        if (stepssize != null && stepssize > 0)
                        {
                           StepSeconds = stepssize.Value;
                        }
                        else
                        {
                            message = "Ungültige Eingabe, Wert unverändert.";
                        }
                        break;
                        
                    case "8": resume = true;
                        break;
                }
            }

            Console.Clear();
            Console.CursorVisible = false;
        }

        private double? ReadDouble(string input) //double? prüft ob wert nicht Null ist
        {
            Console.Write(input);
            string text = Console.ReadLine() ?? "";
            if (double.TryParse(text, out double value))
            {
                return value;
            }
            return null;   // ungültig
        }
    }
}

