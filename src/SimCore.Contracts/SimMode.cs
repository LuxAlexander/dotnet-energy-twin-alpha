namespace SimCore.Contracts;

/// <summary>Betriebszustand der gemeinsamen virtuellen Zeit.</summary>
public enum SimMode
{
    /// <summary>Virtuelle Zeit steht (auch der Ausgangszustand nach dem Start des Masters).</summary>
    Paused,

    /// <summary>Virtuelle Zeit läuft mit <see cref="TimeState.Speed"/>.</summary>
    Running
}
