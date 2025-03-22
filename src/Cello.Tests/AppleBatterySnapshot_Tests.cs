using System.Runtime.Versioning;
using Cello.Apple;

namespace Cello.Tests;

public class AppleBatterySnapshot_Tests
{
    private const string SampleIoregTextFile = "ioreg_sample.txt";

    private readonly string? _sample;

    public AppleBatterySnapshot_Tests()
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
        AppleBatteryState batteryState = AppleSystemBatterySnapshot.CreateSystemSnapshotFromIOReg(new StringReader(_sample)).AppleBatteryState;
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
        AppleBatteryState batteryState = (await AppleSystemBatterySnapshot.CreateSystemSnapshotFromIORegAsync(new StringReader(_sample), TestContext.Current.CancellationToken)).AppleBatteryState;
        Assert.Equal(batteryState.CurrentCapacity, 3005);
        Assert.Equal(batteryState.AppleRawCurrentCapacity, 3005);
        Assert.Equal(batteryState.MaxCapacity, 3531);
        Assert.Equal(batteryState.AppleRawMaxCapacity, 3531);
        Assert.Equal(batteryState.DesignCapacity, 4790);
    }

    [Fact]
    [SupportedOSPlatform("MacOS")]
    public async Task GetAsync_BatteryInfo_Actual_Valid()
    {
        Assert.SkipWhen(!OperatingSystem.IsMacOS(), "ioreg is only supported on macOS");
        var snapshot = await AppleSystemBatterySnapshot.CreateSystemSnapshotFromIORegAsync(TestContext.Current.CancellationToken);
        AssertValid(snapshot.AppleBatteryState);
        BatterySnapshot_Tests.AssertValid(snapshot.GetPrimaryBatteryInfo());
    }

    [Fact]
    [SupportedOSPlatform("MacOS")]
    public void Get_BatteryInfo_Actual_Valid()
    {
        Assert.SkipWhen(!OperatingSystem.IsMacOS(), "ioreg is only supported on macOS");
        var snapshot = AppleSystemBatterySnapshot.CreateSystemSnapshotFromIOReg();
        AssertValid(snapshot.AppleBatteryState);
        BatterySnapshot_Tests.AssertValid(snapshot.GetPrimaryBatteryInfo());
    }

    private static void AssertValid(AppleBatteryState batteryState)
    {
        if (batteryState.BatteryInstalled is true)
        {
            Assert.True(batteryState.DesignCapacity >= 0);
            Assert.True(batteryState.AppleRawMaxCapacity >= 0 && batteryState.AppleRawMaxCapacity <= batteryState.DesignCapacity);
            Assert.True(batteryState.AppleRawCurrentCapacity >= 0 && batteryState.AppleRawCurrentCapacity <= batteryState.AppleRawMaxCapacity);
        }
    }
}
