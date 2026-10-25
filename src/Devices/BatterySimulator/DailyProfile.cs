using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Batterie_Sim
{
    public class DailyProfile
    {
        private readonly BatterieConfig _config;

        private readonly Random random;
        private double noiseFactor = 1.0;
        private int lastWindow = -1;

        /// <param name="random">Zufallsquelle für das Rauschen; mit festem Seed wiederholbar.</param>
        public DailyProfile(BatterieConfig config, Random? random = null)
        {
            _config = config;
            this.random = random ?? new Random();
        }

        public double GetSetpoint(TimeSpan timeOfDay)
        {
            double hour = timeOfDay.TotalHours;   // 0 bis 24
            int windownummer = (int)(timeOfDay.TotalMinutes / 5);

            if (windownummer != lastWindow)
            {
                noiseFactor = 0.9 + 0.2 * random.NextDouble();
                lastWindow = windownummer;
            }
            if (hour >= 7 && hour < 17)
            {
                // Laden: Sinus von Claude
                double setpoint= _config.MaxChargeKW * Math.Sin(Math.PI * (hour - 7) / 10);
                return setpoint * noiseFactor;
            }
            else if (hour >= 18 && hour < 23)
            {
                // Entladen
                double setpoint= -_config.MaxDischargeKW * (0.6 + 0.4 * Math.Exp(-Math.Pow(((hour - 19.5) / 1.5),2)));
                return setpoint*noiseFactor;
            }

            return 0.0;   // alle anderen Zeiten: Standby
        }
    }
}
    

