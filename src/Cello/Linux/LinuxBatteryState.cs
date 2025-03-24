namespace Cello.Linux;

/// <summary>
/// State for a battery on a Linux system.
/// </summary>
public struct LinuxBatteryState
{
    /// <summary>
    /// Battery alarm energy capacity in µWh.
    /// </summary>
    public ulong? Alarm;

    /// <summary>
    /// Capacity in percent.
    /// </summary>
    public double? Capacity;

    /// <summary>
    /// Capacity level.
    /// </summary>
    public string? CapacityLevel;

    /// <summary>
    /// Cycle count.
    /// </summary>
    public int? CycleCount;

    /// <summary>
    /// Current energy capacity in µWh.
    /// </summary>
    public ulong? EnergyNow;

    /// <summary>
    /// Energy capacity at which battery is considered full, in µWh.
    /// </summary>
    public ulong? EnergyFull;

    /// <summary>
    /// Design Energy capacity at which battery is considered full, in µWh.
    /// </summary>
    public ulong? EnergyFullDesign;

    /// <summary>
    /// Battery manufacturer.
    /// </summary>
    public string? Manufacturer;

    /// <summary>
    /// Battery model name.
    /// </summary>
    public string? ModelName;

    /// <summary>
    /// Current power draw in µW.
    /// </summary>
    public ulong? PowerNow;

    /// <summary>
    /// Presence value.
    /// </summary>
    public ulong? Present;

    /// <summary>
    /// Battery serial number.
    /// </summary>
    public string? SerialNumber;

    /// <summary>
    /// Battery status.
    /// </summary>
    public string? Status;

    /// <summary>
    /// Battery technology.
    /// </summary>
    public string? Technology;

    /// <summary>
    /// Temperature of power supply in tenths of degrees Celsius.
    /// </summary>
    public long? Temp;

    /// <summary>
    /// Component type.
    /// </summary>
    public string? Type;

    /// <summary>
    /// Current voltage in µV.
    /// </summary>
    public ulong? VoltageMinDesign;

    /// <summary>
    /// Current voltage in µV.
    /// </summary>
    public ulong? VoltageNow;

    /// <inheritdoc />
    public override string ToString()
    {
        return $$"""
                 {
                     Alarm = {{Alarm?.ToString() ?? "null"}},
                     Capacity = {{Capacity?.ToString() ?? "null"}},
                     CapacityLevel = {{CapacityLevel ?? "null"}},
                     CycleCount = {{CycleCount?.ToString() ?? "null"}},
                     EnergyNow = {{EnergyNow?.ToString() ?? "null"}},
                     EnergyFull = {{EnergyFull?.ToString() ?? "null"}},
                     EnergyFullDesign = {{EnergyFullDesign?.ToString() ?? "null"}},
                     Manufacturer = {{Manufacturer ?? "null"}},
                     ModelName = {{ModelName ?? "null"}},
                     PowerNow = {{PowerNow?.ToString() ?? "null"}},
                     Present = {{Present?.ToString() ?? "null"}},
                     SerialNumber = {{SerialNumber ?? "null"}},
                     Status = {{Status ?? "null"}},
                     Technology = {{Technology ?? "null"}},
                     Type = {{Type ?? "null"}},
                     VoltageMinDesign = {{VoltageMinDesign?.ToString() ?? "null"}},
                     VoltageNow = {{VoltageNow?.ToString() ?? "null"}},
                 }
                 """;
    }
}
