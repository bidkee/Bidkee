using System.Text.Json;

namespace BidkeeClient;

internal static class SessionFile
{
    public static string PathName =>
        System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".bidkee-grok-session.json");

    public static void Save(string? accessToken, string? passNo, string? checkCode)
    {
        var json = JsonSerializer.Serialize(new
        {
            accessToken,
            passNo,
            checkCode,
            savedAt = DateTimeOffset.UtcNow
        });
        File.WriteAllText(PathName, json);
    }

    public static string? ReadAccessToken()
    {
        try
        {
            if (!File.Exists(PathName)) return null;
            using var doc = JsonDocument.Parse(File.ReadAllText(PathName));
            if (doc.RootElement.TryGetProperty("accessToken", out var t))
                return t.GetString();
        }
        catch { }
        return null;
    }
}
