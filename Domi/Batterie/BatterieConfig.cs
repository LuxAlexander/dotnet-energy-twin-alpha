using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Batterie_Sim
{
    public class BatterieConfig
    {
        public string DeviceId { get; set; } = "BAT-01";
        public double CapacityKWh { get; set; } = 20.0;
        public double MaxChargeKW { get; set; } = 5.0;
        public double MaxDischargeKW { get; set; } = 5.0;
        public double SocMinPercent { get; set; } = 10.0;
        public double SocMaxPercent { get; set; } = 100.0;
        public double StartSocPercent { get; set; } = 50.0;
        public double Efficiency { get; set; } = 100.0;
    }
}
