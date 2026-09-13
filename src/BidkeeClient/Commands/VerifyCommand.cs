using System.Text.Json;
using Bidkee.Client;
using Bidkee.Client.Dtos;

namespace BidkeeClient.Commands;

internal static class VerifyCommand
{
    public static async Task<int> RunAsync(BidkeeGateway gateway, string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("verify <checkCode>");
            return 1;
        }
        var r = await gateway.Public.VerifyAsync(new VerifyRequest { CheckCode = args[0] });
        Console.WriteLine(JsonSerializer.Serialize(r, Bidkee.Client.BidkeeJson.Options));
        return r.Ok ? 0 : 1;
    }

    public static async Task<int> OfflineAsync(BidkeeGateway gateway, string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("offline <ticket.json|->");
            return 1;
        }
        var json = args[0] == "-" ? await Console.In.ReadToEndAsync() : await File.ReadAllTextAsync(args[0]);
        var r = await gateway.Public.VerifyOfflineAsync(json);
        Console.WriteLine(JsonSerializer.Serialize(r, Bidkee.Client.BidkeeJson.Options));
        return r.Ok ? 0 : 1;
    }
}
