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
            Assert.True(batteryInfo.MaxChargeCapacity >= 0 && batteryInfo.MaxChargeCapacity <= batteryInfo.DesignChargeCapacity);
            Assert.True(batteryInfo.CurrentChargeCapacity >= 0 && batteryInfo.CurrentChargeCapacity <= batteryInfo.MaxChargeCapacity);
        }
    }

}
