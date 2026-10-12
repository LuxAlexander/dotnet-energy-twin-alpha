using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Batterie_Sim
{
    public class CsvLogger
    {
        private readonly StreamWriter _writer;
        private readonly BatterieConfig _config;

        public CsvLogger(BatterieConfig config)
        {
            _config = config;
            string fileName = $"battery_{config.DeviceId}_{DateTime.Now:yyyy-MM-dd}.csv";
            //string path = Path.Combine(@"..\..\..\docs\data\", fileName);
            _writer = new StreamWriter(fileName);
            _writer.WriteLine("SimTime;DeviceId;Mode;Control;SetpointKW;ActualKW;SOC;TempC;Fault;MaxChargeKW;MaxDischargeKW;SocMin;SocMax");
        }

        public void Log(TimeSpan simTime, Batterie battery, ControlMode control)
        {
            _writer.WriteLine($"{simTime};{_config.DeviceId};{battery.State};{control};{battery.SetpointKW.ToString("F2", CultureInfo.InvariantCulture)};{battery.ActualKW.ToString("F2", CultureInfo.InvariantCulture)};{battery.SocPercent.ToString("F2", CultureInfo.InvariantCulture)};{battery.TemperaturC.ToString("F2", CultureInfo.InvariantCulture)};{battery.ActiveFault};{_config.MaxChargeKW.ToString("F2", CultureInfo.InvariantCulture)};{_config.MaxDischargeKW.ToString("F2", CultureInfo.InvariantCulture)};{_config.SocMinPercent.ToString("F2", CultureInfo.InvariantCulture)};{_config.SocMaxPercent.ToString("F2", CultureInfo.InvariantCulture)}");
        }

        public void Flush() { _writer.Flush(); }
        public void Close() { _writer.Close(); }
    }
}
