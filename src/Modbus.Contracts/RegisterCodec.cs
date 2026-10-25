namespace Modbus.Contracts;

/// <summary>
/// Umrechnung zwischen technischen Werten und 16-Bit-Registern.
/// Mehrteilige Werte: Bytes eines Registers Big-Endian (Modbus-Standard), höherwertiges Wort zuerst.
/// </summary>
public static class RegisterCodec
{
    private static readonly DateTime Epoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

    public static ushort[] EncodeUInt32(uint value) => [(ushort)(value >> 16), (ushort)(value & 0xFFFF)];

    public static uint DecodeUInt32(ushort high, ushort low) => ((uint)high << 16) | low;

    public static ushort[] EncodeFloat32(float value) => EncodeUInt32(BitConverter.SingleToUInt32Bits(value));

    public static float DecodeFloat32(ushort high, ushort low) => BitConverter.UInt32BitsToSingle(DecodeUInt32(high, low));

    /// <summary>Technischer Wert → Registerinhalt (gerundet auf den Datentyp).</summary>
    public static ushort[] Encode(RegisterDefinition definition, double value)
    {
        double raw = value / definition.Scale;
        return definition.Type switch
        {
            RegisterType.UInt16 => [(ushort)Math.Clamp(Math.Round(raw), 0, ushort.MaxValue)],
            RegisterType.UInt32 => EncodeUInt32((uint)Math.Clamp(Math.Round(raw), 0, uint.MaxValue)),
            _ => EncodeFloat32((float)raw)
        };
    }

    /// <summary>Registerinhalt → technischer Wert (Rohwert mal Skalierung).</summary>
    public static double Decode(RegisterDefinition definition, ReadOnlySpan<ushort> registers)
    {
        if (registers.Length < definition.Count)
            throw new ArgumentException($"{definition.Name} braucht {definition.Count} Register.", nameof(registers));

        double raw = definition.Type switch
        {
            RegisterType.UInt16 => (double)registers[0],
            RegisterType.UInt32 => (double)DecodeUInt32(registers[0], registers[1]),   // double, nicht float: sonst verfälscht (Sekunden seit 1970)
            _ => (double)DecodeFloat32(registers[0], registers[1])
        };
        return raw * definition.Scale;
    }

    /// <summary>Virtuelle Zeit als Sekunden seit 1970-01-01 00:00 (ohne Zeitzone, so wie die Simulation sie führt).</summary>
    public static uint ToSeconds(DateTime modelTime) =>
        (uint)Math.Clamp((modelTime - Epoch).TotalSeconds, 0, uint.MaxValue);

    public static DateTime FromSeconds(uint seconds) => Epoch.AddSeconds(seconds);
}
