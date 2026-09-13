using System.Text.Json;
using Bidkee.Client;

namespace BidkeeClient.Commands;

internal static class UploadCommand
{
    public static async Task<int> RunAsync(BidkeeGateway gateway, string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("upload <ticket.json|->");
            return 1;
        }

        string json;
        string name;
        if (args[0] == "-")
        {
            json = await Console.In.ReadToEndAsync();
            name = "stdin.json";
        }
        else
        {
            json = await File.ReadAllTextAsync(args[0]);
            name = Path.GetFileName(args[0]);
        }

        var r = await gateway.Public.UploadAsync(json, name);
        Console.WriteLine(JsonSerializer.Serialize(r, BidkeeJson.Options));
        return r.Ok ? 0 : 1;
    }
}
