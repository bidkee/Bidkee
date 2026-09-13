using Bidkee.Client;
using BidkeeClient.Commands;

namespace BidkeeClient;

public static class Program
{
    static async Task<int> Main(string[] args)
    {
        if (args.Length == 0 || IsHelp(args[0]))
        {
            PrintHelp();
            return 0;
        }

        var options = LoadOptions();
        if (string.IsNullOrWhiteSpace(options.AccessToken))
            options.AccessToken = SessionFile.ReadAccessToken();
        var cmd = args[0].Trim().ToLowerInvariant();
        var rest = args.Skip(1).ToArray();

        try
        {
            using var gateway = BidkeeGateway.Connect(options);

            return cmd switch
            {
                "health" => await HealthCommand.RunAsync(gateway),
                "search" => await SearchCommand.RunAsync(gateway, rest),
                "query" => await SearchCommand.RunAsync(gateway, rest),
                "upload" => await UploadCommand.RunAsync(gateway, rest),
                "ingest" => await UploadCommand.RunAsync(gateway, rest),
                "get" => await SearchCommand.GetAsync(gateway, rest),
                "download" => await GrokCommand.RunAsync(gateway, new[] { "download" }.Concat(rest).ToArray()),
                "resolve" => await GrokCommand.RunAsync(gateway, new[] { "resolve" }.Concat(rest).ToArray()),
                "verify" => await VerifyCommand.RunAsync(gateway, rest),
                "offline" => await VerifyCommand.OfflineAsync(gateway, rest),
                "enroll" => await EnrollCommand.RunAsync(gateway, rest),
                "me" => await EnrollCommand.MeAsync(gateway, rest),
                "present" => await PresentCommand.RunAsync(gateway, rest),
                "revoke" => await PresentCommand.RevokeAsync(gateway, rest),
                "delegate" => await GrokCommand.RunAsync(gateway, new[] { "delegate" }.Concat(rest).ToArray()),
                "gateway" => await GrokCommand.RunAsync(gateway, new[] { "gateway" }),
                "tools" => await GrokCommand.RunAsync(gateway, new[] { "tools" }),
                "capabilities" => await HealthCommand.CapabilitiesAsync(gateway),
                "grok" => await GrokCommand.RunAsync(gateway, rest),
                _ => Unknown(cmd)
            };
        }
        catch (BidkeeException ex)
        {
            Console.Error.WriteLine($"{ex.Code} ({ex.StatusCode}): {ex.Message}");
            if (!string.IsNullOrWhiteSpace(ex.Hint))
                Console.Error.WriteLine(ex.Hint);
            return ex.IsUnauthorized ? 2 : ex.IsRateLimited ? 3 : 1;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    static BidkeeOptions LoadOptions()
    {
        return new BidkeeOptions
        {
            BaseUrl = First(
                Environment.GetEnvironmentVariable("BIDKEE_BASE_URL"),
                ReadJson("BaseUrl"),
                BidkeeOptions.DefaultBaseUrl)!,
            IssuerToken = EmptyToNull(First(
                Environment.GetEnvironmentVariable("BIDKEE_ISSUER_TOKEN"),
                ReadJson("IssuerToken"))),
            AccessToken = EmptyToNull(First(
                Environment.GetEnvironmentVariable("BIDKEE_ACCESS_TOKEN"),
                ReadJson("AccessToken"))),
            OpsApiKey = EmptyToNull(First(
                Environment.GetEnvironmentVariable("BIDKEE_OPS_API_KEY"),
                ReadJson("OpsApiKey")))
        };
    }

    static string? ReadJson(string key)
    {
        try
        {
            var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            if (!File.Exists(path)) return null;
            var text = File.ReadAllText(path);
            var needle = "\"" + key + "\"";
            var i = text.IndexOf(needle, StringComparison.Ordinal);
            if (i < 0) return null;
            var colon = text.IndexOf(':', i);
            var first = text.IndexOf('"', colon + 1);
            var second = text.IndexOf('"', first + 1);
            if (first < 0 || second < 0) return null;
            return text.Substring(first + 1, second - first - 1);
        }
        catch
        {
            return null;
        }
    }

    static string? First(params string?[] values)
    {
        foreach (var v in values)
            if (!string.IsNullOrWhiteSpace(v)) return v;
        return null;
    }

    static string? EmptyToNull(string? s) => string.IsNullOrWhiteSpace(s) ? null : s;

    static bool IsHelp(string s) => s is "-h" or "--help" or "help" or "/?";

    static int Unknown(string cmd)
    {
        Console.Error.WriteLine("unknown command: " + cmd);
        PrintHelp();
        return 1;
    }

    static void PrintHelp()
    {
        Console.WriteLine("""
            BidkeeClient — end-user client (NuGet: Bidkee.Client)

            Usage:
              BidkeeClient search <address|did|checkCode|fileHash>
              BidkeeClient get <checkCode>
              BidkeeClient download <checkCode> [json|b64|did]
              BidkeeClient upload <ticket.json>
              BidkeeClient offline <ticket.json>
              BidkeeClient verify <checkCode>
              BidkeeClient health

            Env:
              BIDKEE_BASE_URL          default https://bidkee.com
              BIDKEE_ISSUER_TOKEN      iss_…
              BIDKEE_ACCESS_TOKEN      agt_…
              BIDKEE_OPS_API_KEY       ops writes only
            """);
    }
}
