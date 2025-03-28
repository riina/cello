﻿namespace Cello;

/// <summary>
/// Battery info.
/// </summary>
public record struct BatteryInfo
{
    /// <summary>
    /// True if the device has a battery.
    /// </summary>
    public bool HasBattery { get; init; }

    /// <summary>
    /// Current charge percentage.
    /// </summary>
    public double? ChargePercentage { get; init; }

    /// <summary>
    /// Charge health percentage.
    /// </summary>
    public double? ChargeHealthPercentage { get; init; }

    /// <summary>
    /// Current charge in mAh.
    /// </summary>
    public BatteryCapacityValue? CurrentChargeCapacity { get; init; }

    /// <summary>
    /// Charge capacity in mAh.
    /// </summary>
    public BatteryCapacityValue? MaxChargeCapacity { get; init; }

    /// <summary>
    /// Design charge capacity in mAh.
    /// </summary>
    public BatteryCapacityValue? DesignChargeCapacity { get; init; }

    /// <summary>
    /// Charge rate in Watts.
    /// </summary>
    /// <remarks>
    /// Positive value indicates the battery is being charged.
    /// Negative value indicates the battery is being discharged.
    /// </remarks>
    public double? ChargeRate { get; init; }

    /// <summary>
    /// Battery voltage in mV.
    /// </summary>
    public double? Voltage { get; init; }

    /// <summary>
    /// Temperature in Celsius.
    /// </summary>
    public double? Temperature { get; init; }

    /// <summary>
    /// Charging flags.
    /// </summary>
    public ChargingFlags? ChargingFlags { get; init; }

    /// <summary>
    /// Time to discharge completion, in seconds.
    /// </summary>
    public double? TimeToDischargeCompletion { get; init; }

    /// <summary>
    /// Time to charge completion, in seconds.
    /// </summary>
    public double? TimeToChargeCompletion { get; init; }
}
