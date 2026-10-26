using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verbraucher
{
    public enum DeviceState
    {
        Off,
        Idle,
        Cooling,
        Fault
    }
    public enum FaultType
    {
        None,
        CompressorFailure,
        OverTemperature
    }
}
