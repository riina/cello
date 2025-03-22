namespace Cello.Tests;

public class BatterySnapshot_Tests
{
    [Fact]
    public async Task CreateSystemSnapshotAsync_Valid()
    {
        Assert.SkipWhen(!SystemBatterySnapshot.IsSystemSnapshotSupported(), "system snapshots are not supported on this OS");
        var snapshot = await SystemBatterySnapshot.CreateSystemSnapshotAsync(TestContext.Current.CancellationToken);
        AssertValid(snapshot.GetPrimaryBatteryInfo());
    }

    [Fact]
    public void CreateSystemSnapshot_Valid()
    {
        Assert.SkipWhen(!SystemBatterySnapshot.IsSystemSnapshotSupported(), "system snapshots are not supported on this OS");
        var snapshot = SystemBatterySnapshot.CreateSystemSnapshot();
        AssertValid(snapshot.GetPrimaryBatteryInfo());
    }

    internal static void AssertValid(BatteryInfo batteryInfo)
    {
        if (batteryInfo.HasBattery)
        {
            Assert.True(batteryInfo.ChargePercentage is >= 0 and <= 100);
            Assert.True(batteryInfo.ChargeHealthPercentage is >= 0 and <= 100);
            {
                if (batteryInfo.MaxChargeCapacity is { } maxChargeCapacity
                    && batteryInfo.DesignChargeCapacity is { } designChargeCapacity
                    && maxChargeCapacity.Units == designChargeCapacity.Units)
                    Assert.True(maxChargeCapacity.Value >= 0 && maxChargeCapacity.Value <= designChargeCapacity.Value);
            }
            {
                if (batteryInfo.CurrentChargeCapacity is { } currentChargeCapacity
                    && batteryInfo.MaxChargeCapacity is { } maxChargeCapacity
                    && currentChargeCapacity.Units == maxChargeCapacity.Units)
                {
                    Assert.True(currentChargeCapacity.Value >= 0 && currentChargeCapacity.Value <= maxChargeCapacity.Value);
                }
            }
        }
    }
}
