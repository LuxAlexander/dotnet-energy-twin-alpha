using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Batterie_Sim
{
    public class ConsoleDashboard
    {
        public ConsoleDashboard()
        {
            Console.CursorVisible = false;
            Console.Clear();
        }

        public void Render(Simulator sim, Batterie battery)
        {
            string bar = BuildBar(battery.SocPercent);

            string[] lines = new string[]
            {
                $" === Batteriespeicher {battery.DeviceId} ===============",
                $" Simulationszeit : {sim.SimTime}   Tempo: x{sim.Speed} Schritt: {sim.StepSeconds} s",
                $"Modus           : {sim.Mode} Zustand: {battery.State}",
                $"Sollwert        :  {battery.SetpointKW} kW",
                $"Ist - Leistung    :  {battery.ActualKW :F2} kW",
                $"SOC             : {bar} {battery.SocPercent :F2}",
                $"Temperatur:  {battery.TemperaturC :F2} °C Störung: {battery.ActiveFault}",
                $"Energie         : geladen {battery.ChargedKWh:F2} entladen {battery.DischargeKWh:F2} kWh Zyklen {battery.EquivalentCycles:F2}",
                $"--------------------------------------------",
                $"[Leertaste] Pause[M] Modus[E] Ein / Aus[Q] Ende"
            };

            Console.SetCursorPosition(0, 0);
            foreach (string line in lines)
            {
                Console.WriteLine(line.PadRight(Console.WindowWidth - 1));  
            }
        }

        private static string BuildBar(double percent, int barWidth = 20)
        {
            int filled = (int)(percent / 100.0 * barWidth);
            filled = Math.Clamp(filled, 0, barWidth);
            return new string('#', filled) + new string('-', barWidth - filled);
        }

        private static string Row(string label, string value) => $"{label,-16}: {value}";
    }
}
