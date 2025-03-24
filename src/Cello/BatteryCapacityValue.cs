namespace Cello;

/// <summary>
/// Represents a capacity in either mAh or mWh.
/// </summary>
/// <remarks>
/// Ideally, everything would be measured in mWh.
/// However, for AppleSmartBattery, I have not identified
/// a way to get this value or an average voltage value
/// (printed on battery) to resolve the value in mWh.
/// </remarks>
public record struct BatteryCapacityValue
{
    /// <summary>
    /// Value.
    /// </summary>
    public double Value { get; init; }

    /// <summary>
    /// Units for this value.
    /// </summary>
    public BatteryCapacityUnits Units { get; init; }

    /// <summary>
    /// Creates an instance of <see cref="BatteryCapacityValue"/> for the passed value as Milliampere-hours (mAh).
    /// </summary>
    /// <param name="value">Value in mAh.</param>
    /// <returns>Value combining numeric value and units.</returns>
    public static BatteryCapacityValue FromMilliampereHours(double value)
    {
        return new BatteryCapacityValue { Value = value, Units = BatteryCapacityUnits.MilliampereHours };
    }

    /// <summary>
    /// Creates an instance of <see cref="BatteryCapacityValue"/> for the passed value as Milliwatt-hours (mWh).
    /// </summary>
    /// <param name="value">Value in mWh.</param>
    /// <returns>Value combining numeric value and units.</returns>
    public static BatteryCapacityValue FromMilliwattHours(double value)
    {
        return new BatteryCapacityValue { Value = value, Units = BatteryCapacityUnits.MilliwattHours };
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Value}{Units switch {
            BatteryCapacityUnits.MilliwattHours => "mWh", //
            BatteryCapacityUnits.MilliampereHours => "mAh", //
            _ => throw new ArgumentOutOfRangeException() //
        }}";
    }
}
