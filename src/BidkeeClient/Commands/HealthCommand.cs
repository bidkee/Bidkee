using System.Text.Json;
using Bidkee.Client;

namespace BidkeeClient.Commands;

internal static class HealthCommand
{
    public static async Task<int> RunAsync(BidkeeGateway gateway)
    {
        var h = await gateway.Public.HealthAsync();
        Console.WriteLine(JsonSerializer.Serialize(h, Bidkee.Client.BidkeeJson.Options));
        if (!string.IsNullOrWhiteSpace(h.Protocol) && h.Protocol != BidkeeOptions.SupportedProtocol)
        {
            Console.Error.WriteLine($"warning: server protocol {h.Protocol}, client {BidkeeOptions.SupportedProtocol}");
            return 4;
        }
        return h.Ok ? 0 : 1;
    }

    public static async Task<int> CapabilitiesAsync(BidkeeGateway gateway)
    {
        var c = await gateway.Agents.GetCapabilitiesAsync();
        Console.WriteLine(JsonSerializer.Serialize(c, Bidkee.Client.BidkeeJson.Options));
        return c.Ok ? 0 : 1;
    }
}
