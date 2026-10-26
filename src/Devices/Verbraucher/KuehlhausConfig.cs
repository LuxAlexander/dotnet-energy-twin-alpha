using System;
using System.Collections.Generic;
using System.Text;

namespace Verbraucher
{
     public class KuehlhausConfig
    {
        public string DeviceId { get; set; } = "Kühlhaus-01";
        public double SollTempC { get; set; } = -20.0;
        public double HystereseK { get; set; } = 2;
        public double WaermekapazitaetKwH { get; set; } = 1.0;
        public double WaermeduchgangKw { get; set; } = 0.05;
        public double KuelleistungKw { get; set; } = 6;
        public double ZusatzwaermekW { get; set; } = 3.0;
        public double AlarmTempC { get; set; } = -10.0;
        public double COP { get; set; } = 2.0;
    }
}
