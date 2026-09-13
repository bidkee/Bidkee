using System.Text.Json;
using Bidkee.Client;
using Bidkee.Client.Grok;

namespace BidkeeClient.Commands;

internal static class GrokCommand
{
    static readonly JsonSerializerOptions Pretty = new(BidkeeJson.Options) { WriteIndented = true };

    public static async Task<int> RunAsync(BidkeeGateway gateway, string[] args)
    {
        if (args.Length == 0 || args[0] is "-h" or "--help" or "help")
        {
            PrintHelp();
            return 0;
        }

        if (string.IsNullOrWhiteSpace(gateway.Options.AccessToken))
            gateway.Options.AccessToken = SessionFile.ReadAccessToken();

        using var grok = new GrokBotClient(gateway);
        var verb = args[0].Trim().ToLowerInvariant();
        var rest = args.Skip(1).ToArray();

        switch (verb)
        {
            case "tools":
                Console.WriteLine(JsonSerializer.Serialize(GrokBotTools.All, Pretty));
                return 0;
            case "health":
                return Print(await grok.HealthAsync());
            case "capabilities":
            case "caps":
                return Print(await grok.CapabilitiesAsync());
            case "gateway":
                return Print(await grok.GatewayCatalogAsync());
            case "enroll":
            {
                var s = await grok.EnrollAsync(Flag(rest, "--agent"), Flag(rest, "--instance"));
                SessionFile.Save(s.AccessToken, s.PassNo, s.CheckCode);
                return Print(new { s.PassNo, s.CheckCode, s.ExpiresAt, s.AccessToken, s.Capabilities, s.Status });
            }
            case "me":
            case "status":
                await grok.AttachAsync();
                return Print(await grok.MeAsync());
            case "search":
                return Print(await grok.SearchAsync(Join(rest)));
            case "lookup":
            case "get":
                Need(rest, "lookup <checkCode>");
                return Print(await grok.LookupAsync(rest[0]));
            case "download":
                Need(rest, "download <checkCode> [json|b64|did]");
                var file = await grok.DownloadAsync(rest[0], rest.Length > 1 ? rest[1] : "json");
                var outPath = rest.Length > 2 ? rest[2] : file.FileName;
                await File.WriteAllBytesAsync(outPath, file.Bytes);
                Console.WriteLine(outPath + " " + file.ContentType + " " + file.Bytes.Length);
                return 0;
            case "resolve":
                Need(rest, "resolve <checkCode>");
                return Print(await grok.ResolveAsync(rest[0]));
            case "verify":
                Need(rest, "verify <checkCode>");
                return Print(await grok.VerifyAsync(rest[0]));
            case "offline":
                Need(rest, "offline <ticket.json>");
                var json = rest[0] == "-" ? await Console.In.ReadToEndAsync() : await File.ReadAllTextAsync(rest[0]);
                return Print(await grok.VerifyOfflineAsync(json));
            case "verify-pass":
            case "pass":
                await grok.AttachAsync();
                return Print(await grok.VerifyPassAsync(Flag(rest, "--pass"), Flag(rest, "--check")));
            case "present":
                await grok.AttachAsync();
                return Print(await grok.PresentAsync());
            case "present-start":
                await grok.AttachAsync();
                return Print(await grok.PresentStartAsync());
            case "present-complete":
                Need(rest, "present-complete <challenge>");
                await grok.AttachAsync();
                return Print(await grok.PresentCompleteAsync(rest[0]));
            case "delegate":
                Need(rest, "delegate <parentPassNo>");
                var child = await grok.DelegateAsync(rest[0], Flag(rest, "--agent"), Flag(rest, "--instance"));
                SessionFile.Save(child.AccessToken, child.PassNo, child.CheckCode);
                return Print(new { child.PassNo, child.CheckCode, child.AccessToken, child.ExpiresAt });
            case "revoke":
                await grok.AttachAsync();
                var r = await grok.RevokeAsync();
                return Print(r);
            default:
                Console.Error.WriteLine("unknown grok verb: " + verb);
                PrintHelp();
                return 1;
        }
    }

    static int Print(object? o)
    {
        Console.WriteLine(JsonSerializer.Serialize(o, Pretty));
        return 0;
    }

    static void Need(string[] rest, string usage)
    {
        if (rest.Length == 0) throw new BidkeeException("MALFORMED", usage, 400);
    }

    static string Join(string[] rest) => string.Join(" ", rest);

    static string? Flag(string[] args, string name)
    {
        for (var i = 0; i < args.Length - 1; i++)
            if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase))
                return args[i + 1];
        return null;
    }

    static void PrintHelp()
    {
        Console.WriteLine("""
            BidkeeClient grok — every route Grok Bot is allowed to call

            Discovery
              grok tools
              grok health
              grok capabilities
              grok gateway

            Identity
              grok enroll [--agent grok-bot] [--instance name]
              grok me
              grok revoke
              grok delegate <parentPassNo> [--agent grok-bot]

            Directory
              grok search <q>
              grok lookup <checkCode>
              grok download <checkCode> [json|b64|did] [out]
              grok resolve <checkCode>

            Verify / present (holder-sign=false)
              grok verify <checkCode>
              grok offline <ticket.json>
              grok verify-pass [--pass BID-…] [--check <hex>]
              grok present
              grok present-start
              grok present-complete <challenge>
            """);
    }
}
