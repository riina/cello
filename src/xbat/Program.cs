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
        var snap = await BatterySnapshot.CreateSystemSnapshotAsync();
        var bi = snap.GetBatteryInfo();
        Console.WriteLine($"charge: {(bi.ChargePercentage is { } chargePercentage ? chargePercentage.ToString("N2") : "n/a")}%");
        Console.WriteLine($"charge health: {(bi.ChargeHealthPercentage is { } chargeHealthPercentage ? chargeHealthPercentage.ToString("N2") : "n/a")}%");
        Console.WriteLine($"[dis]charge rate: {(bi.ChargeRate is { } chargeRate ? chargeRate.ToString("N2") : "n/a")}W");
        Console.WriteLine($"temperature: {(bi.Temperature is { } temperature ? temperature.ToString("N2") : "n/a")}C");
        if (context.ParseResult.GetValueForOption(_details))
        {
            Console.WriteLine($"Details: {snap.GetDetails()}");
        }
    }
}
