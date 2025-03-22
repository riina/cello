using Cello.Apple;
using Cello.Microsoft;

namespace Cello;

/// <summary>
/// Represents a snapshot of the state of system batteries.
/// </summary>
public abstract record SystemBatterySnapshot
{
    /// <summary>
    /// Checks if system snapshots are supported on this platform.
    /// </summary>
    /// <returns>true if system snapshots are supported on this platform.</returns>
    public static bool IsSystemSnapshotSupported()
    {
        return OperatingSystem.IsOSPlatformVersionAtLeast("windows", 5, 1, 2600) || OperatingSystem.IsMacOS();
    }

    /// <summary>
    /// Creates a snapshot of the primary system battery.
    /// </summary>
    /// <returns>Snapshot content.</returns>
    /// <exception cref="PlatformNotSupportedException">Thrown if this member is not supported on the current operating system.</exception>
    public static SystemBatterySnapshot CreateSystemSnapshot()
    {
        if (OperatingSystem.IsOSPlatformVersionAtLeast("windows", 5))
        {
            return WindowsDeviceIoSystemBatterySnapshot.CreateSystemSnapshotFromDeviceIo();
        }
        if (OperatingSystem.IsMacOS())
        {
            return AppleSystemBatterySnapshot.CreateSystemSnapshotFromIOReg();
        }
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    /// Creates a snapshot of the primary system battery.
    /// </summary>
    /// <returns>A <see cref="Task{BatterySnapshot}"/> returning the snapshot content.</returns>
    /// <exception cref="PlatformNotSupportedException">Thrown if this member is not supported on the current operating system.</exception>
    public static async Task<SystemBatterySnapshot> CreateSystemSnapshotAsync(CancellationToken cancellationToken = default)
    {
        if (OperatingSystem.IsOSPlatformVersionAtLeast("windows", 5))
        {
            return WindowsDeviceIoSystemBatterySnapshot.CreateSystemSnapshotFromDeviceIo();
        }
        if (OperatingSystem.IsMacOS())
        {
            return await AppleSystemBatterySnapshot.CreateSystemSnapshotFromIORegAsync(cancellationToken).ConfigureAwait(false);
        }
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    /// Gets the <see cref="BatteryInfo"/> for the primary battery.
    /// </summary>
    /// <returns><see cref="BatteryInfo"/> instance for the primary battery.</returns>
    public abstract BatteryInfo GetPrimaryBatteryInfo();

    /// <summary>
    /// Gets the <see cref="BatteryInfo"/> for all batteries.
    /// </summary>
    /// <returns><see cref="BatteryInfo"/> instances for all present batteries.</returns>
    public abstract List<BatteryInfo> GetBatteryInfos();

    /// <summary>
    /// Gets summary text about this snapshot.
    /// </summary>
    /// <returns>Summary text about the snapshot.</returns>
    public abstract string GetDetails();
}
