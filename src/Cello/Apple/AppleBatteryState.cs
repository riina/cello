using System.Diagnostics;
using System.Runtime.Versioning;

namespace Cello.Apple;

/// <summary>
/// State for a battery in an Apple Mac device.
/// </summary>
public struct AppleBatteryState
{
    /// <summary>
    /// Raw battery current capacity in mAh.
    /// </summary>
    public int? AppleRawCurrentCapacity;

    /// <summary>
    /// Raw battery max capacity in mAh.
    /// </summary>
    public int? AppleRawMaxCapacity;

    /// <summary>
    /// Raw battery voltage in mV.
    /// </summary>
    public int? AppleRawBatteryVoltage;

    /// <summary>
    /// Current capacity relative to <see cref="MaxCapacity"/>.
    /// </summary>
    public int? CurrentCapacity;

    /// <summary>
    /// Nominal charge capacity in mAh.
    /// </summary>
    public int? NominalChargeCapacity;

    /// <summary>
    /// Max capacity value.
    /// </summary>
    public int? MaxCapacity;

    /// <summary>
    /// Design capacity.
    /// </summary>
    public int? DesignCapacity;

    /// <summary>
    /// Voltage value.
    /// </summary>
    public int? Voltage;

    /// <summary>
    /// True if battery is charging.
    /// </summary>
    public bool? IsCharging;

    /// <summary>
    /// True if battery is installed.
    /// </summary>
    public bool? BatteryInstalled;

    /// <summary>
    /// True if battery charge is at a critical level.
    /// </summary>
    public bool? AtCriticalLevel;

    /// <summary>
    /// Virtual temperature.
    /// </summary>
    public int? VirtualTemperature;

    /// <summary>
    /// Current amperage in mA.
    /// </summary>
    public long? Amperage;

    /// <summary>
    /// Charge cycle count.
    /// </summary>
    public long? CycleCount;

    /// <summary>
    /// An external power source is connected.
    /// </summary>
    public bool? ExternalConnected;

    /// <summary>
    /// External power source is capable of charging the battery.
    /// </summary>
    public bool? ExternalChargeCapable;

    /// <summary>
    /// Last time state was updated.
    /// </summary>
    public ulong? UpdateTime;

    /// <summary>
    /// Design charge cycle count.
    /// </summary>
    public int? DesignCycleCount9C;

    /// <summary>
    /// True if fully charged.
    /// </summary>
    public bool? FullyCharged;

    /// <summary>
    /// Time remaining for current operation, or -1 if no operation is occurring.
    /// </summary>
    /// <remarks>
    /// Operations include battery depletion to empty and charging.
    /// This value will be -1 in cases such as deferring charge completion based on a schedule.
    /// </remarks>
    public short? TimeRemaining;

    /// <summary>
    /// Expected time remaining to complete charging in minutes, or -1 if not charging.
    /// </summary>
    public short? AvgTimeToFull;

    /// <summary>
    /// Expected time remaining to complete discharge in minutes, or -1 if not discharging.
    /// </summary>
    public short? AvgTimeToEmpty;

    /// <summary>
    /// Initializes an instance of <see cref="AppleBatteryState"/>.
    /// </summary>
    public AppleBatteryState()
    {
    }

    internal void Update(IORegProperty property)
    {
        IORegHelper.TrySet(property, "AppleRawCurrentCapacity", ref AppleRawCurrentCapacity);
        IORegHelper.TrySet(property, "AppleRawMaxCapacity", ref AppleRawMaxCapacity);
        IORegHelper.TrySet(property, "AppleRawBatteryVoltage", ref AppleRawBatteryVoltage);
        IORegHelper.TrySet(property, "CurrentCapacity", ref CurrentCapacity);
        IORegHelper.TrySet(property, "NominalChargeCapacity", ref NominalChargeCapacity);
        IORegHelper.TrySet(property, "MaxCapacity", ref MaxCapacity);
        IORegHelper.TrySet(property, "DesignCapacity", ref DesignCapacity);
        IORegHelper.TrySet(property, "Voltage", ref Voltage);
        IORegHelper.TrySet(property, "IsCharging", ref IsCharging);
        IORegHelper.TrySet(property, "BatteryInstalled", ref BatteryInstalled);
        IORegHelper.TrySet(property, "AtCriticalLevel", ref AtCriticalLevel);
        IORegHelper.TrySet(property, "VirtualTemperature", ref VirtualTemperature);
        IORegHelper.TrySet(property, "Amperage", ref Amperage);
        IORegHelper.TrySet(property, "CycleCount", ref CycleCount);
        IORegHelper.TrySet(property, "ExternalConnected", ref ExternalConnected);
        IORegHelper.TrySet(property, "ExternalChargeCapable", ref ExternalChargeCapable);
        IORegHelper.TrySet(property, "UpdateTime", ref UpdateTime);
        IORegHelper.TrySet(property, "DesignCycleCount9C", ref DesignCycleCount9C);
        IORegHelper.TrySet(property, "FullyCharged", ref FullyCharged);
        IORegHelper.TrySet(property, "TimeRemaining", ref TimeRemaining);
        IORegHelper.TrySet(property, "AvgTimeToFull", ref AvgTimeToFull);
        IORegHelper.TrySet(property, "AvgTimeToEmpty", ref AvgTimeToEmpty);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return @$"{{
    AppleRawCurrentCapacity = {AppleRawCurrentCapacity?.ToString() ?? "null"},
    AppleRawMaxCapacity = {AppleRawMaxCapacity?.ToString() ?? "null"},
    AppleRawBatteryVoltage = {AppleRawBatteryVoltage?.ToString() ?? "null"},
    CurrentCapacity = {CurrentCapacity?.ToString() ?? "null"},
    NominalChargeCapacity = {NominalChargeCapacity?.ToString() ?? "null"},
    MaxCapacity = {MaxCapacity?.ToString() ?? "null"},
    DesignCapacity = {DesignCapacity?.ToString() ?? "null"},
    Voltage = {Voltage?.ToString() ?? "null"},
    IsCharging = {IsCharging?.ToString() ?? "null"},
    BatteryInstalled = {BatteryInstalled?.ToString() ?? "null"},
    AtCriticalLevel = {AtCriticalLevel?.ToString() ?? "null"},
    VirtualTemperature = {VirtualTemperature?.ToString() ?? "null"},
    Amperage = {Amperage?.ToString() ?? "null"},
    CycleCount = {CycleCount?.ToString() ?? "null"},
    ExternalConnected = {ExternalConnected?.ToString() ?? "null"},
    ExternalChargeCapable = {ExternalChargeCapable?.ToString() ?? "null"},
    UpdateTime = {(UpdateTime != null ? $"{UpdateTime.ToString()} /* {DateTimeOffset.FromUnixTimeSeconds((long)UpdateTime).ToString()} */" : "null")},
    DesignCycleCount9C = {DesignCycleCount9C?.ToString() ?? "null"},
    TimeRemaining = {TimeRemaining?.ToString() ?? "null"},
    AvgTimeToFull = {AvgTimeToFull?.ToString() ?? "null"},
    AvgTimeToEmpty = {AvgTimeToEmpty?.ToString() ?? "null"},
}}";
    }

    /// <summary>
    /// Converts this instance to a <see cref="BatteryInfo"/>.
    /// </summary>
    /// <returns><see cref="BatteryInfo"/> instance summarizing battery state.</returns>
    public BatteryInfo GetBatteryInfo()
    {
        double? chargePercentage = null;
        double? chargeHealthPercentage = null;
        double? chargeRate = null;
        double? temperature = null;
        {
            if (CurrentCapacity is { } currentCapacity && MaxCapacity is { } maxCapacity)
            {
                chargePercentage = (double)currentCapacity / maxCapacity * 100.0;
            }
        }
        {
            if (AppleRawMaxCapacity is { } maxCapacity && DesignCapacity is { } designCapacity)
            {
                chargeHealthPercentage = (double)maxCapacity / designCapacity * 100.0;
            }
        }
        {
            if (AppleRawBatteryVoltage is { } batteryVoltage && Amperage is { } amperage)
            {
                chargeRate = batteryVoltage * amperage / 1_000_000.0;
            }
        }
        {
            if (VirtualTemperature is { } virtualTemperature)
            {
                temperature = virtualTemperature / 100.0;
            }
        }
        ChargingFlags chargingFlags = ChargingFlags.None;
        if (ExternalConnected is true)
        {
            chargingFlags |= ChargingFlags.ExternalPowerConnected;
        }
        if (IsCharging is true)
        {
            chargingFlags |= ChargingFlags.ExternalPowerCharging;
        }
        if (Amperage < 0)
        {
            chargingFlags |= ChargingFlags.Discharging;
        }
        return new BatteryInfo
        {
            HasBattery = BatteryInstalled ?? false,
            ChargePercentage = chargePercentage,
            ChargeHealthPercentage = chargeHealthPercentage,
            CurrentChargeCapacity = AppleRawCurrentCapacity,
            MaxChargeCapacity = AppleRawMaxCapacity,
            DesignChargeCapacity = DesignCapacity,
            ChargeRate = chargeRate,
            Temperature = temperature,
            TimeToDischargeCompletion = AvgTimeToEmpty >= 0 ? AvgTimeToEmpty : null,
            TimeToChargeCompletion = AvgTimeToFull >= 0 ? AvgTimeToFull : null,
            ChargingFlags = chargingFlags
        };
    }
}
