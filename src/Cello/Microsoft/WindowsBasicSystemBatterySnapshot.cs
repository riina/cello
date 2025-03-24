using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.System.Power;

namespace Cello.Microsoft;

/// <summary>
/// Represents a basic snapshot of the state of system batteries a Windows system.
/// </summary>
public record WindowsBasicSystemBatterySnapshot : SystemBatterySnapshot
{
    internal SYSTEM_POWER_STATUS SystemPowerStatus { get; init; }

    [SupportedOSPlatform("windows5.1.2600")]
    internal WindowsBasicSystemBatterySnapshot(SYSTEM_POWER_STATUS systemPowerStatus)
    {
        SystemPowerStatus = systemPowerStatus;
    }

    private static BatteryInfo CreateBatteryInfo(SYSTEM_POWER_STATUS systemPowerStatus)
    {
        ChargingFlags chargingFlags = ChargingFlags.None;
        if ((systemPowerStatus.BatteryFlag & 8) != 0)
        {
            chargingFlags |= ChargingFlags.ExternalPowerCharging;
        }
        if (systemPowerStatus.ACLineStatus == 1)
        {
            chargingFlags |= ChargingFlags.ExternalPowerConnected;
        }
        if ((chargingFlags & (ChargingFlags.ExternalPowerCharging | ChargingFlags.ExternalPowerConnected)) == 0)
        {
            chargingFlags |= ChargingFlags.Discharging;
        }
        return new BatteryInfo
        {
            HasBattery = (systemPowerStatus.BatteryFlag & 128) == 0, //
            ChargePercentage = systemPowerStatus.BatteryLifePercent, //
            ChargingFlags = chargingFlags, //
            TimeToDischargeCompletion = systemPowerStatus.BatteryLifeTime != uint.MaxValue ? systemPowerStatus.BatteryLifeTime : null
        };
    }

    /// <inheritdoc />
    public override BatteryInfo GetPrimaryBatteryInfo()
    {
        return CreateBatteryInfo(SystemPowerStatus);
    }

    /// <inheritdoc />
    public override List<BatteryInfo> GetBatteryInfos()
    {
        return [CreateBatteryInfo(SystemPowerStatus)];
    }

    /// <inheritdoc />
    public override string GetDetails()
    {
        return $$"""
                 {
                     ACLineStatus = {{SystemPowerStatus.ACLineStatus}},
                     BatteryFlag = {{SystemPowerStatus.BatteryFlag}},
                     BatteryFullLifeTime = {{SystemPowerStatus.BatteryFullLifeTime}},
                     BatteryLifePercent = {{SystemPowerStatus.BatteryLifePercent}},
                     BatteryLifeTime = {{SystemPowerStatus.BatteryLifeTime}},
                     SystemStatusFlag = {{SystemPowerStatus.SystemStatusFlag}},
                 }
                 """;
    }

    /// <summary>
    /// Creates a snapshot of the primary system battery using the Windows API.
    /// </summary>
    /// <returns>Snapshot content.</returns>
    /// <exception cref="Win32Exception">Thrown if failed to get system power status.</exception>
    [SupportedOSPlatform("windows5.1.2600")]
    public static WindowsBasicSystemBatterySnapshot CreateSystemSnapshotFromSystemPowerStatus()
    {
        if (!PInvoke.GetSystemPowerStatus(out var systemPowerStatus))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
        return new WindowsBasicSystemBatterySnapshot(systemPowerStatus);
    }
}
