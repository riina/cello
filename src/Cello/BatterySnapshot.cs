namespace Cello;

/// <summary>
/// Represents a snapshot of the state of a battery.
/// </summary>
public abstract record BatterySnapshot
{
    /// <summary>
    /// Creates a snapshot of the primary system battery.
    /// </summary>
    /// <returns>Snapshot content.</returns>
    /// <exception cref="PlatformNotSupportedException">Thrown if this member is not supported on the current operating system.</exception>
    public static BatterySnapshot CreateSystemSnapshot()
    {
        if (OperatingSystem.IsMacOS())
        {
            return AppleBatteryState.CreateSystemSnapshotFromIOReg();
        }
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    /// Creates a snapshot of the primary system battery.
    /// </summary>
    /// <returns>A <see cref="Task{BatterySnapshot}"/> returning the snapshot content.</returns>
    /// <exception cref="PlatformNotSupportedException">Thrown if this member is not supported on the current operating system.</exception>
    public static async Task<BatterySnapshot> CreateSystemSnapshotAsync(CancellationToken cancellationToken = default)
    {
        if (OperatingSystem.IsMacOS())
        {
            return await AppleBatteryState.CreateSystemSnapshotFromIORegAsync(cancellationToken).ConfigureAwait(false);
        }
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    /// Converts this instance to a <see cref="BatteryInfo"/>.
    /// </summary>
    /// <returns><see cref="BatteryInfo"/> instance summarizing battery state.</returns>
    public abstract BatteryInfo GetBatteryInfo();

    /// <summary>
    /// Gets summary text about this snapshot.
    /// </summary>
    /// <returns>Summary text about the snapshot.</returns>
    public abstract string GetDetails();
}
