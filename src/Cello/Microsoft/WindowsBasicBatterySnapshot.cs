using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.System.Power;

namespace Cello.Microsoft;

/// <summary>
/// Basic snapshot on a Windows system.
/// </summary>
public record WindowsBasicBatterySnapshot : BatterySnapshot
{
    [SupportedOSPlatform("windows5.1.2600")]
    internal WindowsBasicBatterySnapshot(SYSTEM_POWER_STATUS systemPowerStatus)
    {
        _systemPowerStatus = systemPowerStatus;
    }

    internal SYSTEM_POWER_STATUS _systemPowerStatus { get; init; }

    /// <inheritdoc />
    public override BatteryInfo GetBatteryInfo()
    {
        ChargingFlags chargingFlags = ChargingFlags.None;
        if ((_systemPowerStatus.BatteryFlag & 8) != 0)
        {
            chargingFlags |= ChargingFlags.ExternalPowerCharging;
        }
        if (_systemPowerStatus.ACLineStatus == 1)
        {
            chargingFlags |= ChargingFlags.ExternalPowerConnected;
        }
        if ((chargingFlags & (ChargingFlags.ExternalPowerCharging | ChargingFlags.ExternalPowerConnected)) == 0)
        {
            chargingFlags |= ChargingFlags.Discharging;
        }
        return new BatteryInfo
        {
            HasBattery = (_systemPowerStatus.BatteryFlag & 128) == 0, //
            ChargePercentage = _systemPowerStatus.BatteryLifePercent, //
            ChargingFlags = chargingFlags, //
            TimeToDischargeCompletion = _systemPowerStatus.BatteryLifeTime != uint.MaxValue ? _systemPowerStatus.BatteryLifeTime : null
        };
    }

    /// <inheritdoc />
    public override string GetDetails()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Creates a snapshot of the primary system battery using the Windows API.
    /// </summary>
    /// <returns>Snapshot content.</returns>
    /// <exception cref="Win32Exception">Thrown if failed to get system power status.</exception>
    [SupportedOSPlatform("windows5.1.2600")]
    public static WindowsBasicBatterySnapshot CreateSystemSnapshotFromSystemPowerStatus()
    {
        if (!PInvoke.GetSystemPowerStatus(out var systemPowerStatus))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
        return new WindowsBasicBatterySnapshot(systemPowerStatus);
    }
}
