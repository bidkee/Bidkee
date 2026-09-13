using Bidkee.Client.Grok;
using Xunit;

namespace Bidkee.Client.Tests;

public class GrokBotToolsTests
{
    [Fact]
    public void Catalog_covers_agent_channel()
    {
        var names = GrokBotTools.All.Select(t => t.Name).ToHashSet();
        foreach (var n in new[]
        {
            "bidkee_health", "bidkee_capabilities", "bidkee_gateway",
            "bidkee_enroll", "bidkee_me", "bidkee_search", "bidkee_lookup",
            "bidkee_download", "bidkee_resolve", "bidkee_verify",
            "bidkee_verify_offline", "bidkee_verify_pass", "bidkee_present",
            "bidkee_present_start", "bidkee_present_complete",
            "bidkee_delegate", "bidkee_revoke"
        })
            Assert.Contains(n, names);
    }
}
