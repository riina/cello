using System.Diagnostics;
using System.Runtime.Versioning;

namespace Cello;

/// <summary>
/// State for a battery in an Apple Mac device.
/// </summary>
public struct AppleBatteryState
{
    /// <summary>
    /// Raw battery current capacity in mAh.
    /// </summary>
    public int? AppleRawCurrentCapacity;

    /// <summary>
    /// Raw battery max capacity in mAh.
    /// </summary>
    public int? AppleRawMaxCapacity;

    /// <summary>
    /// Raw battery voltage in mV.
    /// </summary>
    public int? AppleRawBatteryVoltage;

    /// <summary>
    /// Current capacity relative to <see cref="MaxCapacity"/>.
    /// </summary>
    public int? CurrentCapacity;

    /// <summary>
    /// Nominal charge capacity in mAh.
    /// </summary>
    public int? NominalChargeCapacity;

    /// <summary>
    /// Max capacity value.
    /// </summary>
    public int? MaxCapacity;

    /// <summary>
    /// Design capacity.
    /// </summary>
    public int? DesignCapacity;

    /// <summary>
    /// Voltage value.
    /// </summary>
    public int? Voltage;

    /// <summary>
    /// True if battery is charging.
    /// </summary>
    public bool? IsCharging;

    /// <summary>
    /// True if battery is installed.
    /// </summary>
    public bool? BatteryInstalled;

    /// <summary>
    /// True if battery charge is at a critical level.
    /// </summary>
    public bool? AtCriticalLevel;

    /// <summary>
    /// Virtual temperature.
    /// </summary>
    public int? VirtualTemperature;

    /// <summary>
    /// Current amperage in mA.
    /// </summary>
    public long? Amperage;

    /// <summary>
    /// Charge cycle count.
    /// </summary>
    public long? CycleCount;

    /// <summary>
    /// An external power source is connected.
    /// </summary>
    public bool? ExternalConnected;

    /// <summary>
    /// External power source is capable of charging the battery.
    /// </summary>
    public bool? ExternalChargeCapable;

    /// <summary>
    /// Last time state was updated.
    /// </summary>
    public ulong? UpdateTime;

    /// <summary>
    /// Design charge cycle count.
    /// </summary>
    public int? DesignCycleCount9C;

    /// <summary>
    /// True if fully charged.
    /// </summary>
    public bool? FullyCharged;

    /// <summary>
    /// Initializes an instance of <see cref="AppleBatteryState"/>.
    /// </summary>
    public AppleBatteryState()
    {
    }

    private void Update(IORegProperty property)
    {
        TrySet(property, "AppleRawCurrentCapacity", ref AppleRawCurrentCapacity);
        TrySet(property, "AppleRawMaxCapacity", ref AppleRawMaxCapacity);
        TrySet(property, "AppleRawBatteryVoltage", ref AppleRawBatteryVoltage);
        TrySet(property, "CurrentCapacity", ref CurrentCapacity);
        TrySet(property, "NominalChargeCapacity", ref NominalChargeCapacity);
        TrySet(property, "MaxCapacity", ref MaxCapacity);
        TrySet(property, "DesignCapacity", ref DesignCapacity);
        TrySet(property, "Voltage", ref Voltage);
        TrySet(property, "IsCharging", ref IsCharging);
        TrySet(property, "BatteryInstalled", ref BatteryInstalled);
        TrySet(property, "AtCriticalLevel", ref AtCriticalLevel);
        TrySet(property, "VirtualTemperature", ref VirtualTemperature);
        TrySet(property, "Amperage", ref Amperage);
        TrySet(property, "CycleCount", ref CycleCount);
        TrySet(property, "ExternalConnected", ref ExternalConnected);
        TrySet(property, "ExternalChargeCapable", ref ExternalChargeCapable);
        TrySet(property, "UpdateTime", ref UpdateTime);
        TrySet(property, "DesignCycleCount9C", ref DesignCycleCount9C);
        TrySet(property, "FullyCharged", ref FullyCharged);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return @$"{{
    AppleRawCurrentCapacity = {AppleRawCurrentCapacity?.ToString() ?? "null"},
    AppleRawMaxCapacity = {AppleRawMaxCapacity?.ToString() ?? "null"},
    AppleRawBatteryVoltage = {AppleRawBatteryVoltage?.ToString() ?? "null"},
    CurrentCapacity = {CurrentCapacity?.ToString() ?? "null"},
    NominalChargeCapacity = {NominalChargeCapacity?.ToString() ?? "null"},
    MaxCapacity = {MaxCapacity?.ToString() ?? "null"},
    DesignCapacity = {DesignCapacity?.ToString() ?? "null"},
    Voltage = {Voltage?.ToString() ?? "null"},
    IsCharging = {IsCharging?.ToString() ?? "null"},
    BatteryInstalled = {BatteryInstalled?.ToString() ?? "null"},
    AtCriticalLevel = {AtCriticalLevel?.ToString() ?? "null"},
    VirtualTemperature = {VirtualTemperature?.ToString() ?? "null"},
    Amperage = {Amperage?.ToString() ?? "null"},
    CycleCount = {CycleCount?.ToString() ?? "null"},
    ExternalConnected = {ExternalConnected?.ToString() ?? "null"},
    ExternalChargeCapable = {ExternalChargeCapable?.ToString() ?? "null"},
    UpdateTime = {(UpdateTime != null ? $"{UpdateTime.ToString()} /* {DateTimeOffset.FromUnixTimeSeconds((long)UpdateTime).ToString()} */" : "null")},
    DesignCycleCount9C = {DesignCycleCount9C?.ToString() ?? "null"},
    FullyCharged = {FullyCharged?.ToString() ?? "null"},
}}";
    }

    /// <summary>
    /// Converts this instance to a <see cref="BatteryInfo"/>.
    /// </summary>
    /// <returns><see cref="BatteryInfo"/> instance summarizing battery state.</returns>
    public BatteryInfo GetBatteryInfo()
    {
        double? chargePercentage = null;
        double? chargeHealthPercentage = null;
        double? chargeRate = null;
        double? temperature = null;
        {
            if (CurrentCapacity is { } currentCapacity && MaxCapacity is { } maxCapacity)
            {
                chargePercentage = (double)currentCapacity / maxCapacity * 100.0;
            }
        }
        {
            if (AppleRawMaxCapacity is { } maxCapacity && DesignCapacity is { } designCapacity)
            {
                chargeHealthPercentage = (double)maxCapacity / designCapacity * 100.0;
            }
        }
        {
            if (AppleRawBatteryVoltage is { } batteryVoltage && Amperage is { } amperage)
            {
                chargeRate = batteryVoltage * amperage / 1_000_000.0;
            }
        }
        {
            if (VirtualTemperature is { } virtualTemperature)
            {
                temperature = virtualTemperature / 100.0;
            }
        }
        return new BatteryInfo(
            chargePercentage,
            chargeHealthPercentage,
            AppleRawCurrentCapacity,
            AppleRawMaxCapacity,
            DesignCapacity,
            chargeRate,
            temperature);
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
    public static AppleBatterySnapshot CreateSystemSnapshotFromIOReg()
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
    /// <returns>A <see cref="Task{AppleBatterySnapshot}"/> returning the snapshot content.</returns>
    [SupportedOSPlatform("MacOS")]
    public static async Task<AppleBatterySnapshot> CreateSystemSnapshotFromIORegAsync(CancellationToken cancellationToken = default)
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
    public static AppleBatterySnapshot CreateSystemSnapshotFromIOReg(TextReader ioRegTextReader)
    {
        AppleBatteryState pending = new();
        ProcessIOReg(ioRegTextReader, (_, obj, property) =>
        {
            if (obj.Name.Span is "AppleSmartBattery")
            {
                pending.Update(property);
            }
        });
        return new AppleBatterySnapshot(pending);
    }

    /// <summary>
    /// Creates a snapshot of the primary system battery using output from ioreg utility.
    /// </summary>
    /// <returns>A <see cref="Task{AppleBatterySnapshot}"/> returning the snapshot content.</returns>
    public static async Task<AppleBatterySnapshot> CreateSystemSnapshotFromIORegAsync(TextReader ioRegTextReader, CancellationToken cancellationToken = default)
    {
        AppleBatteryState pending = new();
        await ProcessIORegAsync(ioRegTextReader, (_, obj, property) =>
        {
            if (obj.Name.Span is "AppleSmartBattery")
            {
                pending.Update(property);
            }
        }, cancellationToken);
        return new AppleBatterySnapshot(pending);
    }

    private static void TrySet(IORegProperty property, ReadOnlySpan<char> name, ref int? value, bool withReparseToNegative = true)
    {
        if (property.Name.Span.SequenceEqual(name))
        {
            if (!int.TryParse(property.Value.Span, out int valueM))
            {
                if (withReparseToNegative && uint.TryParse(property.Value.Span, out uint valueUM))
                {
                    value = (int)valueUM;
                    return;
                }
                throw new InvalidDataException($"Invalid {name}");
            }
            value = valueM;
        }
    }

    private static void TrySet(IORegProperty property, ReadOnlySpan<char> name, ref long? value, bool withReparseToNegative = true)
    {
        if (property.Name.Span.SequenceEqual(name))
        {
            if (!long.TryParse(property.Value.Span, out long valueM))
            {
                if (withReparseToNegative && ulong.TryParse(property.Value.Span, out ulong valueUM))
                {
                    value = (long)valueUM;
                    return;
                }
                throw new InvalidDataException($"Invalid {name}");
            }
            value = valueM;
        }
    }

    private static void TrySet(IORegProperty property, ReadOnlySpan<char> name, ref ulong? value)
    {
        if (property.Name.Span.SequenceEqual(name))
        {
            if (!ulong.TryParse(property.Value.Span, out ulong valueM))
            {
                throw new InvalidDataException($"Invalid {name}");
            }
            value = valueM;
        }
    }

    private static void TrySet(IORegProperty property, ReadOnlySpan<char> name, ref bool? value)
    {
        if (property.Name.Span.SequenceEqual(name))
        {
            switch (property.Value.Span)
            {
                case "Yes":
                    value = true;
                    return;
                case "No":
                    value = false;
                    return;
            }

            throw new InvalidDataException($"Invalid {name}");
        }
    }

    private readonly record struct IORegObject(ReadOnlyMemory<char> Name, ReadOnlyMemory<char> Id);

    private readonly record struct IORegProperty(ReadOnlyMemory<char> Name, ReadOnlyMemory<char> Value);

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
