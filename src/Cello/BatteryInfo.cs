namespace Cello;

/// <summary>
/// Battery info.
/// </summary>
public record struct BatteryInfo
{
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
    public int? CurrentChargeCapacity { get; init; }

    /// <summary>
    /// Charge capacity in mAh.
    /// </summary>
    public int? MaxChargeCapacity { get; init; }

    /// <summary>
    /// Design charge capacity in mAh.
    /// </summary>
    public int? DesignChargeCapacity { get; init; }

    /// <summary>
    /// Charge rate in Watts.
    /// </summary>
    /// <remarks>
    /// Positive value indicates the battery is being charged.
    /// Negative value indicates the battery is being discharged.
    /// </remarks>
    public double? ChargeRate { get; init; }

    /// <summary>
    /// Temperature in Celsius.
    /// </summary>
    public double? Temperature { get; init; }

    /// <summary>
    /// Initializes an instance of <see cref="BatteryInfo"/>.
    /// </summary>
    /// <param name="chargePercentage">Current charge percentage.</param>
    /// <param name="chargeHealthPercentage">Charge health percentage.</param>
    /// <param name="currentChargeCapacity">Current charge in mAh.</param>
    /// <param name="maxChargeCapacity">Charge capacity in mAh.</param>
    /// <param name="designChargeCapacity">Design charge capacity in mAh.</param>
    /// <param name="chargeRate">Charge rate in Watts.</param>
    /// <param name="temperature">Temperature in Celsius.</param>
    public BatteryInfo(
        double? chargePercentage,
        double? chargeHealthPercentage,
        int? currentChargeCapacity,
        int? maxChargeCapacity,
        int? designChargeCapacity,
        double? chargeRate,
        double? temperature)
    {
        ChargePercentage = chargePercentage;
        ChargeHealthPercentage = chargeHealthPercentage;
        CurrentChargeCapacity = currentChargeCapacity;
        MaxChargeCapacity = maxChargeCapacity;
        DesignChargeCapacity = designChargeCapacity;
        ChargeRate = chargeRate;
        Temperature = temperature;
    }
}
