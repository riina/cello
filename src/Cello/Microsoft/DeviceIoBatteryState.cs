namespace Cello.Microsoft;

/// <summary>
/// Represents the state of a battery from Windows DeviceIoControl.
/// </summary>
public struct DeviceIoBatteryState
{
    /// <summary>
    /// If true, capacity values are relative.
    /// </summary>
    public bool CapacityIsRelative;

    /// <summary>
    /// Current capacity in mWh (or undefined units if <see cref="CapacityIsRelative"/> is true), or null if not vailable.
    /// </summary>
    public uint? Capacity;

    /// <summary>
    /// Charge rate in mW (or undefined units if <see cref="CapacityIsRelative"/> is set), or null if not available.
    /// </summary>
    public int? Rate;

    /// <summary>
    /// Theoretical capacity of battery when new in mWh (or undefined units if <see cref="CapacityIsRelative"/> is true).
    /// </summary>
    public uint DesignedCapacity;

    /// <summary>
    /// Current fully charged capacity of battery in mWh (or undefined units if <see cref="CapacityIsRelative"/> is true).
    /// </summary>
    public uint FullChargedCapacity;

    /// <summary>
    /// Temperature in tenths of degrees Kelvin.
    /// </summary>
    public uint Temperature;

    /// <summary>
    /// Estimated time to complete discharge in seconds, or null if not available.
    /// </summary>
    public uint? EstimatedTime;

    /// <summary>
    /// Current voltage across battery terminals in mV, or null if not available.
    /// </summary>
    public uint? Voltage;

    /// <summary>
    /// Charging flags.
    /// </summary>
    public ChargingFlags ChargingFlags;
}
