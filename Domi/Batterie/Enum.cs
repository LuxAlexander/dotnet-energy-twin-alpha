using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Batterie_Sim
{
  public enum DeviceState
    {
        Off, 
        Standby, 
        Charging, 
        Discharging, 
        Fault, 
        Cooldown
    }

    public enum ControlMode
    {
        Auto,
        Manual
    }
    public enum FaultType
    {
        None,
        EmergencyStop, 
        Overheat
    }
}
