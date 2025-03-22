using System.ComponentModel;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Devices.DeviceAndDriverInstallation;
using Windows.Win32.Foundation;

namespace Cello.Microsoft;

/// <summary>
/// Represents a snapshot of the state of system batteries a Windows system.
/// </summary>
public record WindowsDeviceIoSystemBatterySnapshot : SystemBatterySnapshot
{
    /// <inheritdoc />
    public override BatteryInfo GetPrimaryBatteryInfo()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override List<BatteryInfo> GetBatteryInfos()
    {
        throw new NotImplementedException();
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
    [SupportedOSPlatform("windows5.0")]
    public static WindowsDeviceIoSystemBatterySnapshot CreateSystemSnapshotFromDeviceIo()
    {
        var hdev = PInvoke.SetupDiGetClassDevs(
            PInvoke.GUID_DEVCLASS_BATTERY,
            null,
            HWND.Null,
            SETUP_DI_GET_CLASS_DEVS_FLAGS.DIGCF_PRESENT | SETUP_DI_GET_CLASS_DEVS_FLAGS.DIGCF_DEVICEINTERFACE);
        throw new NotImplementedException();
    }
}
