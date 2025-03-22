using System.CommandLine;
using System.CommandLine.Invocation;
using Cello;

var command = new XBatCommand("a", "b");
await command.InvokeAsync(args);

class XBatCommand : Command
{
    private readonly Option<bool> _details;

    public XBatCommand(string name, string? description = null) : base(name, description)
    {
        AddOption(_details = new Option<bool>(["-d", "--details"], "Display tracked battery details"));
        this.SetHandler(HandleAsync);
    }

    private async Task HandleAsync(InvocationContext context)
    {
        var snap = await SystemBatterySnapshot.CreateSystemSnapshotAsync();
        var bi = snap.GetPrimaryBatteryInfo();
        await using var stream = Console.OpenStandardOutput();
        if (bi.HasBattery)
        {
            Console.WriteLine($"charge: {(bi.ChargePercentage is { } chargePercentage ? chargePercentage.ToString("N2") : "n/a")}%");
            Console.WriteLine($"charge health: {(bi.ChargeHealthPercentage is { } chargeHealthPercentage ? chargeHealthPercentage.ToString("N2") : "n/a")}%");
            Console.WriteLine($"[dis]charge rate: {(bi.ChargeRate is { } chargeRate ? chargeRate.ToString("N2") : "n/a")}W");
            Console.WriteLine($"voltage: {(bi.Voltage is { } voltage ? voltage.ToString("N2") : "n/a")}mV");
            Console.WriteLine($"temperature: {(bi.Temperature is { } temperature ? temperature.ToString("N2") : "n/a")}C");
            {
                Console.WriteLine($"charge status: {(bi.ChargingFlags is { } chargingFlags ? GetChargeStatus(chargingFlags) : "n/a")}");
            }
            {
                if (bi.ChargingFlags is { } chargingFlags && (chargingFlags & ChargingFlags.ExternalPowerCharging) != 0)
                {
                    Console.WriteLine($"time to full: {(bi.TimeToChargeCompletion is { } timeToChargeCompletion ? TimeSpan.FromSeconds(timeToChargeCompletion).ToString() : "n/a")}");
                }
            }
            {
                if (bi.ChargingFlags is { } chargingFlags && (chargingFlags & ChargingFlags.Discharging) != 0)
                {
                    Console.WriteLine($"time to empty: {(bi.TimeToDischargeCompletion is { } timeToDischargeCompletion ? TimeSpan.FromSeconds(timeToDischargeCompletion).ToString() : "n/a")}");
                }
            }
            if (context.ParseResult.GetValueForOption(_details))
            {
                Console.WriteLine($"Details: {snap.GetDetails()}");
            }
        }
        else
        {
            Console.WriteLine("no battery installed");
        }
    }

    private static string GetChargeStatus(ChargingFlags chargingFlags)
    {
        if ((chargingFlags & ChargingFlags.ExternalPowerCharging) != 0)
        {
            return "charging";
        }
        if ((chargingFlags & ChargingFlags.ExternalPowerConnected) != 0)
        {
            return "plugged in";
        }
        if ((chargingFlags & ChargingFlags.Discharging) != 0)
        {
            return "discharging";
        }
        return "n/a";
    }
}
