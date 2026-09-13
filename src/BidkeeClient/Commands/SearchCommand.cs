using System.Text.Json;
using Bidkee.Client;

namespace BidkeeClient.Commands;

internal static class SearchCommand
{
    public static async Task<int> RunAsync(BidkeeGateway gateway, string[] args)
    {
        var q = args.Length == 0 ? "" : string.Join(" ", args);
        var r = await gateway.Public.SearchAsync(q);
        Console.WriteLine(JsonSerializer.Serialize(r, Bidkee.Client.BidkeeJson.Options));
        return 0;
    }

    public static async Task<int> GetAsync(BidkeeGateway gateway, string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("get <checkCode>");
            return 1;
        }
        var d = await gateway.Public.GetAsync(args[0]);
        Console.WriteLine(JsonSerializer.Serialize(d, Bidkee.Client.BidkeeJson.Options));
        return 0;
    }
}
