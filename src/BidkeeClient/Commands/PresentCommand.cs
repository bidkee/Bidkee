using System.Text.Json;
using Bidkee.Client;

namespace BidkeeClient.Commands;

internal static class PresentCommand
{
    public static async Task<int> RunAsync(BidkeeGateway gateway, string[] args)
    {
        var session = await Open(gateway);
        var r = await session.PresentAsync();
        Console.WriteLine(JsonSerializer.Serialize(r, Bidkee.Client.BidkeeJson.Options));
        return r.Allowed ? 0 : 1;
    }

    public static async Task<int> RevokeAsync(BidkeeGateway gateway, string[] args)
    {
        var session = await Open(gateway);
        var r = await session.RevokeAsync();
        Console.WriteLine(JsonSerializer.Serialize(r, Bidkee.Client.BidkeeJson.Options));
        return r.Ok ? 0 : 1;
    }

    static async Task<Bidkee.Client.Agent.BidkeeAgentSession> Open(BidkeeGateway gateway)
    {
        if (!string.IsNullOrWhiteSpace(gateway.Options.AccessToken))
            return await gateway.Agents.ResumeAsync(gateway.Options.AccessToken!);
        return await gateway.Agents.EnrollAsync(new Bidkee.Client.Dtos.AgentEnrollRequest
        {
            AgentId = "grok-bot",
            InstanceId = Environment.MachineName,
            Purpose = ["search", "verify", "present"]
        });
    }
}
