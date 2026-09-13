using System.Text.Json;
using Bidkee.Client;
using Bidkee.Client.Dtos;

namespace BidkeeClient.Commands;

internal static class EnrollCommand
{
    public static async Task<int> RunAsync(BidkeeGateway gateway, string[] args)
    {
        var agentId = Flag(args, "--agent") ?? "grok-bot";
        var instance = Flag(args, "--instance") ?? Environment.MachineName;
        using var session = await gateway.Agents.EnrollAsync(new AgentEnrollRequest
        {
            AgentId = agentId,
            InstanceId = instance,
            TtlHours = 24,
            Purpose = ["search", "verify", "present"]
        });
        SessionFile.Save(session.AccessToken, session.PassNo, session.CheckCode);
        Console.WriteLine(JsonSerializer.Serialize(new
        {
            session.PassNo,
            session.CheckCode,
            session.ExpiresAt,
            session.Status,
            session.Issuer,
            accessToken = session.AccessToken,
            session.Capabilities
        }, Bidkee.Client.BidkeeJson.Options));
        return 0;
    }

    public static async Task<int> MeAsync(BidkeeGateway gateway, string[] args)
    {
        var token = gateway.Options.AccessToken ?? SessionFile.ReadAccessToken();
        if (string.IsNullOrWhiteSpace(token))
        {
            Console.Error.WriteLine("set BIDKEE_ACCESS_TOKEN or run: BidkeeClient grok enroll");
            return 2;
        }
        using var session = await gateway.Agents.ResumeAsync(token);
        var me = await session.MeAsync();
        Console.WriteLine(JsonSerializer.Serialize(me, Bidkee.Client.BidkeeJson.Options));
        return me.Ok ? 0 : 1;
    }

    static string? Flag(string[] args, string name)
    {
        for (var i = 0; i < args.Length - 1; i++)
            if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase))
                return args[i + 1];
        return null;
    }
}
