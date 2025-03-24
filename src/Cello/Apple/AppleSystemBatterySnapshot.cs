using System.Diagnostics;
using System.Runtime.Versioning;

namespace Cello.Apple;

/// <summary>
/// Represents a snapshot of the state of system batteries on an Apple Mac.
/// </summary>
/// <param name="AppleBatteryState">Raw Apple Mac battery state data.</param>
public record AppleSystemBatterySnapshot(AppleBatteryState AppleBatteryState) : SystemBatterySnapshot
{
    /// <inheritdoc />
    public override BatteryInfo GetPrimaryBatteryInfo()
    {
        return AppleBatteryState.GetBatteryInfo();
    }

    /// <inheritdoc />
    public override List<BatteryInfo> GetBatteryInfos()
    {
        return [AppleBatteryState.GetBatteryInfo()];
    }

    /// <inheritdoc />
    public override string GetDetails()
    {
        return AppleBatteryState.ToString();
    }

    [SupportedOSPlatform("MacOS")]
    private static Process StartIORegProcess()
    {
        ProcessStartInfo psi = new() { FileName = "ioreg", UseShellExecute = false, Arguments = "-c AppleSmartBattery -w0", RedirectStandardOutput = true };
        Process? p = Process.Start(psi);
        if (p == null) throw new IOException("Failed to start ioreg process");
        return p;
    }

    /// <summary>
    /// Creates a snapshot of the primary system battery using the ioreg utility.
    /// </summary>
    /// <returns>Snapshot content.</returns>
    [SupportedOSPlatform("MacOS")]
    public static AppleSystemBatterySnapshot CreateSystemSnapshotFromIOReg()
    {
        using Process p = StartIORegProcess();
        using StreamReader sr = p.StandardOutput;
        MemoryStream ms = new();
        sr.BaseStream.CopyTo(ms);
        p.WaitForExit();
        ms.Position = 0;
        using StreamReader srr = new(ms);
        return CreateSystemSnapshotFromIOReg(srr);
    }

    /// <summary>
    /// Creates a snapshot of the primary system battery using the ioreg utility.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="Task{AppleBatterySnapshot}"/> returning the snapshot content.</returns>
    [SupportedOSPlatform("MacOS")]
    public static async Task<AppleSystemBatterySnapshot> CreateSystemSnapshotFromIORegAsync(CancellationToken cancellationToken = default)
    {
        using Process p = StartIORegProcess();
        using StreamReader sr = p.StandardOutput;
        MemoryStream ms = new();
        await sr.BaseStream.CopyToAsync(ms, cancellationToken);
        await p.WaitForExitAsync(cancellationToken);
        ms.Position = 0;
        using StreamReader srr = new(ms);
        return await CreateSystemSnapshotFromIORegAsync(srr, cancellationToken);
    }

    /// <summary>
    /// Creates a snapshot of the primary system battery using output from ioreg utility.
    /// </summary>
    /// <returns>Snapshot content.</returns>
    public static AppleSystemBatterySnapshot CreateSystemSnapshotFromIOReg(TextReader ioRegTextReader)
    {
        AppleBatteryState pending = new();
        ProcessIOReg(ioRegTextReader, (_, obj, property) =>
        {
            if (obj.Name.Span is "AppleSmartBattery")
            {
                pending.Update(property);
            }
        });
        return new AppleSystemBatterySnapshot(pending);
    }

    /// <summary>
    /// Creates a snapshot of the primary system battery using output from ioreg utility.
    /// </summary>
    /// <returns>A <see cref="Task{AppleBatterySnapshot}"/> returning the snapshot content.</returns>
    public static async Task<AppleSystemBatterySnapshot> CreateSystemSnapshotFromIORegAsync(TextReader ioRegTextReader, CancellationToken cancellationToken = default)
    {
        AppleBatteryState pending = new();
        await ProcessIORegAsync(ioRegTextReader, (_, obj, property) =>
        {
            if (obj.Name.Span is "AppleSmartBattery")
            {
                pending.Update(property);
            }
        }, cancellationToken);
        return new AppleSystemBatterySnapshot(pending);
    }

    private delegate void PropertyProcessDelegate(IReadOnlyList<IORegObject> stack, IORegObject obj,
        IORegProperty property);

    private static void ProcessIOReg(TextReader ioRegTextReader, PropertyProcessDelegate del)
    {
        List<IORegObject> objects = new();
        bool obj = false;
        while (ioRegTextReader.ReadLine() is { } line)
        {
            ProcessIORegLine(objects, ref obj, line, del);
        }
    }

    private static async Task ProcessIORegAsync(TextReader ioRegTextReader, PropertyProcessDelegate del, CancellationToken cancellationToken)
    {
        List<IORegObject> objects = new();
        bool obj = false;
        while (await ioRegTextReader.ReadLineAsync(cancellationToken) is { } line)
        {
            ProcessIORegLine(objects, ref obj, line, del);
        }
    }

    private static void ProcessIORegLine(List<IORegObject> objects, ref bool obj, string line, PropertyProcessDelegate del)
    {
        if (!TryGetIORegDepth(line, out int depth)) return;
        if (obj)
        {
            if (line[depth] == '}')
            {
                obj = false;
                return;
            }

            int nameIdx = depth + 1;
            ReadOnlyMemory<char> name = line.AsMemory(nameIdx);
            int endNameIdx = name.Span.IndexOf('"');
            if (endNameIdx == -1) throw new InvalidDataException();
            if (objects.Count == 0) return;
            name = name[..endNameIdx];
            int valueIdx = nameIdx + endNameIdx + 4;
            ReadOnlyMemory<char> value = line.AsMemory(valueIdx);
            del(objects, objects[^1], new IORegProperty(name, value));
        }
        else
        {
            switch (line[depth])
            {
                case '{':
                    obj = true;
                    break;
                case '+':
                    int nDepth = depth / 2, ndd = objects.Count - nDepth;
                    if (ndd > 0) objects.RemoveRange(nDepth, ndd);
                    int nameIdx = depth + 4;
                    ReadOnlyMemory<char> name = line.AsMemory(nameIdx);
                    int endNameIdx = name.Span.IndexOf(' ');
                    if (endNameIdx == -1) throw new InvalidDataException();
                    name = name[..endNameIdx];
                    int idIdx = nameIdx + endNameIdx + 9;
                    ReadOnlyMemory<char> id = line.AsMemory(idIdx);
                    int endIdIdx = id.Span.IndexOf(' ');
                    if (endIdIdx == -1) throw new InvalidDataException();
                    id = id[..endIdIdx];
                    objects.Add(new IORegObject(name, id));
                    break;
                default:
                    throw new InvalidDataException();
            }
        }
    }

    private static bool TryGetIORegDepth(string line, out int depth)
    {
        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (c != ' ' && c != '|')
            {
                depth = i;
                return true;
            }
        }

        depth = line.Length;
        return false;
    }
}
