using System.Runtime.Versioning;

namespace Cello.Tests;

public class AppleBatteryState_Tests
{
    private const string SampleIoregTextFile = "ioreg_sample.txt";

    private readonly string? _sample;

    public AppleBatteryState_Tests()
    {
        try
        {
            _sample = File.ReadAllText(SampleIoregTextFile);
        }
        catch
        {
            // ignored
        }
    }

    [Fact]
    public void Get_Ioreg_Sample_MatchesExpected()
    {
        if (_sample == null) throw new Exception($"Failed to find sample file {SampleIoregTextFile}");
        AppleBatteryState batteryState = AppleBatteryState.CreateSystemSnapshotFromIOReg(new StringReader(_sample)).AppleBatteryState;
        Assert.Equal(batteryState.CurrentCapacity, 3005);
        Assert.Equal(batteryState.AppleRawCurrentCapacity, 3005);
        Assert.Equal(batteryState.MaxCapacity, 3531);
        Assert.Equal(batteryState.AppleRawMaxCapacity, 3531);
        Assert.Equal(batteryState.DesignCapacity, 4790);
    }

    [Fact]
    public async Task GetAsync_Ioreg_Sample_MatchesExpected()
    {
        if (_sample == null) throw new Exception($"Failed to find sample file {SampleIoregTextFile}");
        AppleBatteryState batteryState = (await AppleBatteryState.CreateSystemSnapshotFromIORegAsync(new StringReader(_sample), TestContext.Current.CancellationToken)).AppleBatteryState;
        Assert.Equal(batteryState.CurrentCapacity, 3005);
        Assert.Equal(batteryState.AppleRawCurrentCapacity, 3005);
        Assert.Equal(batteryState.MaxCapacity, 3531);
        Assert.Equal(batteryState.AppleRawMaxCapacity, 3531);
        Assert.Equal(batteryState.DesignCapacity, 4790);
    }

    [Fact]
    [SupportedOSPlatform("MacOS")]
    public async Task GetAsync_Ioreg_Actual_Valid()
    {
        var snapshot = await BatterySnapshot.CreateSystemSnapshotAsync(TestContext.Current.CancellationToken);
        AssertValid(snapshot.GetBatteryInfo());
    }

    [Fact]
    [SupportedOSPlatform("MacOS")]
    public async Task GetAsync_BatteryInfo_Actual_Valid()
    {
        var snapshot = await AppleBatteryState.CreateSystemSnapshotFromIORegAsync(TestContext.Current.CancellationToken);
        AssertValid(snapshot.AppleBatteryState);
        AssertValid(snapshot.GetBatteryInfo());
    }

    [Fact]
    [SupportedOSPlatform("MacOS")]
    public void Get_Ioreg_Actual_Valid()
    {
        var snapshot = BatterySnapshot.CreateSystemSnapshot();
        AssertValid(snapshot.GetBatteryInfo());
    }

    [Fact]
    [SupportedOSPlatform("MacOS")]
    public void Get_BatteryInfo_Actual_Valid()
    {
        var snapshot = AppleBatteryState.CreateSystemSnapshotFromIOReg();
        AssertValid(snapshot.AppleBatteryState);
        AssertValid(snapshot.GetBatteryInfo());
    }

    private void AssertValid(AppleBatteryState batteryState)
    {
        Assert.True(batteryState.DesignCapacity >= 0);
        Assert.True(batteryState.AppleRawMaxCapacity >= 0 && batteryState.AppleRawMaxCapacity <= batteryState.DesignCapacity);
        Assert.True(batteryState.AppleRawCurrentCapacity >= 0 && batteryState.AppleRawCurrentCapacity <= batteryState.AppleRawMaxCapacity);
    }

    private void AssertValid(BatteryInfo batteryInfo)
    {
        Assert.True(batteryInfo.ChargePercentage is >= 0 and <= 100);
        Assert.True(batteryInfo.ChargeHealthPercentage is >= 0 and <= 100);
        Assert.True(batteryInfo.MaxChargeCapacity >= 0 && batteryInfo.MaxChargeCapacity <= batteryInfo.DesignChargeCapacity);
        Assert.True(batteryInfo.CurrentChargeCapacity >= 0 && batteryInfo.CurrentChargeCapacity <= batteryInfo.MaxChargeCapacity);
    }
}
