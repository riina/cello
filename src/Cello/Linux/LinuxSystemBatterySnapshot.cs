using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Globalization;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;

namespace Cello.Linux;

/// <summary>
/// Represents a snapshot of the state of system batteries on a Linux system.
/// </summary>
public partial record LinuxSystemBatterySnapshot : SystemBatterySnapshot
{
    private const string PowerSupplyDirectory = "/sys/class/power_supply";

    [GeneratedRegex(@"^BAT\d+")]
    private static partial Regex GetBatteryRegex();

    [GeneratedRegex(@"^\s*(?<key>\w+)\s*=\s*(?<value>.*)$")]
    private static partial Regex GetUEventLineRegex();

    private ImmutableList<LinuxBatteryState> BatteryStates { get; init; }

    /// <summary>
    /// Initializes an instance of <see cref="LinuxSystemBatterySnapshot"/>.
    /// </summary>
    /// <param name="batteryStates">Battery states.</param>
    public LinuxSystemBatterySnapshot(ImmutableList<LinuxBatteryState> batteryStates)
    {
        BatteryStates = batteryStates;
    }

    /// <inheritdoc />
    public override BatteryInfo GetPrimaryBatteryInfo()
    {
        if (BatteryStates.Count == 0)
        {
            return new BatteryInfo { HasBattery = false };
        }
        return CreateBatteryInfo(BatteryStates[0]);
    }

    /// <inheritdoc />
    public override List<BatteryInfo> GetBatteryInfos()
    {
        return [..BatteryStates.Select(CreateBatteryInfo)];
    }

    /// <inheritdoc />
    public override string GetDetails()
    {
        return new StringBuilder("[").Append(Environment.NewLine).AppendJoin($",{Environment.NewLine}", BatteryStates.Select(static s => s.ToString())).Append(Environment.NewLine).Append(']').ToString();
    }

    private BatteryInfo CreateBatteryInfo(LinuxBatteryState linuxBatteryState)
    {
        ChargingFlags chargingFlags = ChargingFlags.None;
        switch (linuxBatteryState.Status)
        {
            case "Charging":
                chargingFlags |= ChargingFlags.ExternalPowerConnected;
                chargingFlags |= ChargingFlags.ExternalPowerCharging;
                break;
            case "NotCharging":
                chargingFlags |= ChargingFlags.ExternalPowerConnected;
                break;
            case "Discharging":
                chargingFlags |= ChargingFlags.Discharging;
                break;
            case "FULL":
                break;
        }
        double? chargePercentage = null;
        double? chargeHealthPercentage = null;
        double? chargeRate = null;
        double? timeToDischargeCompletion = null;
        double? timeToChargeCompletion = null;
        {
            if (linuxBatteryState.Capacity is { } capacity)
            {
                chargePercentage = capacity;
            }
            else if (linuxBatteryState is { EnergyNow: { } energyNow, EnergyFull: { } energyFull })
            {
                chargePercentage = (double)energyNow / energyFull * 100.0;
            }
        }
        {
            if (linuxBatteryState is { EnergyFull: { } energyFull, EnergyFullDesign: { } energyFullDesign })
            {
                chargeHealthPercentage = (double)energyFull / energyFullDesign * 100.0;
            }
        }
        {
            if (linuxBatteryState.PowerNow is { } powerNow)
            {
                if ((chargingFlags & ChargingFlags.ExternalPowerCharging) != 0)
                {
                    chargeRate = powerNow / 1_000_000.0;
                    if (powerNow > 0 && linuxBatteryState is { EnergyFull: { } energyFull, EnergyNow: { } energyNow })
                    {
                        timeToChargeCompletion = (double)(Math.Max(energyFull, energyNow) - energyNow) / powerNow * (60.0 * 60.0);
                    }
                }
                else
                {
                    chargeRate = -(powerNow / 1_000_000.0);
                    if (powerNow > 0 && linuxBatteryState.EnergyNow is { } energyNow)
                    {
                        timeToDischargeCompletion = (double)energyNow / powerNow * (60.0 * 60.0);
                    }
                }
            }
        }
        {
            return new BatteryInfo
            {
                HasBattery = true, //
                ChargePercentage = chargePercentage, //
                ChargeHealthPercentage = chargeHealthPercentage, //
                CurrentChargeCapacity = linuxBatteryState.EnergyNow is { } energyNow ? BatteryCapacityValue.FromMilliwattHours(energyNow / 1000.0) : null, //
                MaxChargeCapacity = linuxBatteryState.EnergyFull is { } energyFull ? BatteryCapacityValue.FromMilliwattHours(energyFull / 1000.0) : null, //
                DesignChargeCapacity = linuxBatteryState.EnergyFullDesign is { } energyFullDesign ? BatteryCapacityValue.FromMilliwattHours(energyFullDesign / 1000.0) : null, //
                ChargeRate = chargeRate, //
                Voltage = linuxBatteryState.VoltageNow is { } voltageNow ? voltageNow / 1000.0 : null, //
                ChargingFlags = chargingFlags, //
                TimeToDischargeCompletion = timeToDischargeCompletion, //
                TimeToChargeCompletion = timeToChargeCompletion, //
            };
        }
    }

    /// <summary>
    /// Creates a snapshot of the system battery status.
    /// </summary>
    /// <returns>Snapshot content.</returns>
    [SupportedOSPlatform("linux")]
    public static LinuxSystemBatterySnapshot CreateSystemSnapshotFromPowerSupplyDirectory()
    {
        List<LinuxBatteryState> batteryStates = new();
        foreach (string dir in Directory.GetDirectories(PowerSupplyDirectory))
        {
            if (GetBatteryRegex().Match(Path.GetFileName(dir)) is not { Success: true })
            {
                continue;
            }
            if (LoadStateFromPowerSupplyDirectoryEntry(dir) is { } batteryStateValue)
            {
                batteryStates.Add(batteryStateValue);
            }
        }
        return new LinuxSystemBatterySnapshot(batteryStates.ToImmutableList());
    }

    /// <summary>
    /// Creates a snapshot of the system battery status.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Snapshot content.</returns>
    [SupportedOSPlatform("linux")]
    public static async Task<LinuxSystemBatterySnapshot> CreateSystemSnapshotFromPowerSupplyDirectoryAsync(CancellationToken cancellationToken = default)
    {
        ConcurrentBag<LinuxBatteryState> batteryStates = new();
        await Parallel.ForEachAsync(Directory.GetDirectories(PowerSupplyDirectory),
            cancellationToken,
            async (dir, subCancellation) =>
            {
                if (GetBatteryRegex().Match(Path.GetFileName(dir)) is not { Success: true })
                {
                    return;
                }
                if (await LoadStateFromPowerSupplyDirectoryEntryAsync(dir, subCancellation) is { } batteryStateValue)
                {
                    batteryStates.Add(batteryStateValue);
                }
            });
        return new LinuxSystemBatterySnapshot(batteryStates.ToImmutableList());
    }

    private static LinuxBatteryState? LoadStateFromPowerSupplyDirectoryEntry(string directory)
    {
        string ueventFile = Path.Join(directory, "uevent");
        if (File.Exists(ueventFile))
        {
            using var reader = File.OpenText(ueventFile);
            var fallbackDictionary = new Dictionary<string, string>();
            foreach (string checkProperty in s_checkProperties)
            {
                string checkPropertyFile = Path.Join(directory, checkProperty);
                if (File.Exists(checkPropertyFile))
                {
                    fallbackDictionary.Add(checkProperty, File.ReadAllText(checkPropertyFile));
                }
            }
            return LoadStateFromUEventAndFallback(PopulateUEventFile(reader), fallbackDictionary);
        }
        var dictionary = new Dictionary<string, string>();
        foreach (string checkProperty in s_checkProperties)
        {
            string checkPropertyFile = Path.Join(directory, checkProperty);
            if (File.Exists(checkPropertyFile))
            {
                dictionary.Add(checkProperty, File.ReadAllText(checkPropertyFile));
            }
        }
        return LoadStateFromValueMap(dictionary);
    }

    private static async Task<LinuxBatteryState?> LoadStateFromPowerSupplyDirectoryEntryAsync(string directory, CancellationToken cancellationToken = default)
    {
        string ueventFile = Path.Join(directory, "uevent");
        if (File.Exists(ueventFile))
        {
            using var reader = File.OpenText(ueventFile);
            var fallbackDictionary = new ConcurrentDictionary<string, string>();
            await Parallel.ForEachAsync(s_checkPropertiesWithUEvent, cancellationToken, async (checkProperty, subCancellation) =>
            {
                string checkPropertyFile = Path.Join(directory, checkProperty);
                if (File.Exists(checkPropertyFile))
                {
                    fallbackDictionary.TryAdd(checkProperty, await File.ReadAllTextAsync(checkPropertyFile, subCancellation));
                }
            });
            return LoadStateFromUEventAndFallback(await PopulateUEventFileAsync(reader, cancellationToken), fallbackDictionary);
        }
        var dictionary = new ConcurrentDictionary<string, string>();
        await Parallel.ForEachAsync(s_checkProperties, cancellationToken, async (checkProperty, subCancellation) =>
        {
            string checkPropertyFile = Path.Join(directory, checkProperty);
            if (File.Exists(checkPropertyFile))
            {
                dictionary.TryAdd(checkProperty, await File.ReadAllTextAsync(checkPropertyFile, subCancellation));
            }
        });
        return LoadStateFromValueMap(dictionary);
    }

    private static Dictionary<string, string> PopulateUEventFile(TextReader textReader)
    {
        Dictionary<string, string> result = new();
        var regex = GetUEventLineRegex();
        int lineIndex = 0;
        while (textReader.ReadLine() is { } line)
        {
            if (regex.Match(line) is not { Success: true } match)
            {
                throw new InvalidDataException($"Unexpected line format in uevent (line {lineIndex}): {line}");
            }
            result[match.Groups["key"].Value] = match.Groups["value"].Value;
            lineIndex++;
        }
        return result;
    }

    private static async Task<Dictionary<string, string>> PopulateUEventFileAsync(TextReader textReader, CancellationToken cancellationToken)
    {
        Dictionary<string, string> result = new();
        var regex = GetUEventLineRegex();
        int lineIndex = 0;
        while (await textReader.ReadLineAsync(cancellationToken) is { } line)
        {
            if (regex.Match(line) is not { Success: true } match)
            {
                throw new InvalidDataException($"Unexpected line format in uevent (line {lineIndex}): {line}");
            }
            result[match.Groups["key"].Value] = match.Groups["value"].Value;
            lineIndex++;
        }
        return result;
    }

    private static LinuxBatteryState? LoadStateFromUEventAndFallback(IReadOnlyDictionary<string, string> dictionary, IReadOnlyDictionary<string, string> fallback)
    {
        if (!dictionary.TryGetValue("DEVTYPE", out string? devType))
        {
            return null;
        }
        if (devType != "power_supply")
        {
            return null;
        }
        const string prefix = "POWER_SUPPLY_";
        var subDictionary = new Dictionary<string, string>(dictionary
            .Where(static pair => pair.Key.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
            .Select(static pair => new KeyValuePair<string, string>(pair.Key[prefix.Length..].ToLowerInvariant(), pair.Value)));
        foreach (var pair in fallback)
        {
            subDictionary.TryAdd(pair.Key, pair.Value);
        }
        return LoadStateFromValueMap(subDictionary);
    }

    private static LinuxBatteryState LoadStateFromValueMap(IReadOnlyDictionary<string, string> dictionary)
    {
        return new LinuxBatteryState
        {
            Alarm = ReadValueOrFallback<ulong>(dictionary, "alarm"), //
            Capacity = ReadValueOrFallback<double>(dictionary, "capacity"), //
            CapacityLevel = dictionary.GetValueOrDefault("capacity_level"), //
            CycleCount = ReadValueOrFallback<int>(dictionary, "cycle_count"), //
            EnergyNow = ReadValueOrFallback<ulong>(dictionary, "energy_now"), //
            EnergyFull = ReadValueOrFallback<ulong>(dictionary, "energy_full"), //
            EnergyFullDesign = ReadValueOrFallback<ulong>(dictionary, "energy_full_design"), //
            Manufacturer = dictionary.GetValueOrDefault("manufacturer"), //
            ModelName = dictionary.GetValueOrDefault("model_name"), //
            PowerNow = ReadValueOrFallback<ulong>(dictionary, "power_now"), //
            Present = ReadValueOrFallback<ulong>(dictionary, "present"), //
            SerialNumber = dictionary.GetValueOrDefault("serial_number"), //
            Status = dictionary.GetValueOrDefault("status"), //
            Technology = dictionary.GetValueOrDefault("technology"), //
            Type = dictionary.GetValueOrDefault("type"), //
            VoltageMinDesign = ReadValueOrFallback<ulong>(dictionary, "voltage_min_design"), //
            VoltageNow = ReadValueOrFallback<ulong>(dictionary, "voltage_now"), //
        };
    }

    private static readonly string[] s_checkPropertiesWithUEvent =
    [
        "alarm", //
    ];

    private static readonly string[] s_checkProperties =
    [
        "alarm", //
        "capacity", //
        "capacity_level", //
        "cycle_count", //
        "energy_full", //
        "energy_full_design", //
        "energy_now", //
        "manufacturer", //
        "model_name", //
        "power_now", //
        "present", //
        "serial_number", //
        "status", //
        "technology", //
        "type", //
        "voltage_min_design", //
        "voltage_now", //
    ];

    private static T? ReadValueOrFallback<T>(IReadOnlyDictionary<string, string> dictionary, string key, T? fallback = null) where T : struct, IParsable<T>
    {
        if (!dictionary.TryGetValue(key, out string? value))
        {
            return fallback;
        }
        if (!T.TryParse(value, CultureInfo.InvariantCulture, out T parsedValue))
        {
            return fallback;
        }
        return parsedValue;
    }
}
