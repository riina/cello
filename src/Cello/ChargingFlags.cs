namespace Cello;

/// <summary>
/// Flags for charging status.
/// </summary>
[Flags]
public enum ChargingFlags
{
    /// <summary>
    /// No flags.
    /// </summary>
    None = 0,

    /// <summary>
    /// Discharging.
    /// </summary>
    Discharging = 1 << 0,

    /// <summary>
    /// External power is connected.
    /// </summary>
    ExternalPowerConnected = 1 << 1,

    /// <summary>
    /// External power is charging the device.
    /// </summary>
    ExternalPowerCharging = 1 << 2,
}
