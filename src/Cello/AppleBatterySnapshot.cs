namespace Cello;

/// <summary>
/// Snapshot of an Apple Mac battery.
/// </summary>
/// <param name="AppleBatteryState">Raw Apple Mac battery state data.</param>
public record AppleBatterySnapshot(AppleBatteryState AppleBatteryState) : BatterySnapshot
{
    /// <inheritdoc />
    public override BatteryInfo GetBatteryInfo()
    {
        return AppleBatteryState.GetBatteryInfo();
    }

    /// <inheritdoc />
    public override string GetDetails()
    {
        return AppleBatteryState.ToString();
    }
}
