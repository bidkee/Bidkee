using Bidkee.Client;
using Bidkee.Client.Auth;
using Xunit;

namespace Bidkee.Client.Tests;

public class OptionsAndTokenTests
{
    [Fact]
    public void BaseUri_appends_slash()
    {
        var o = new BidkeeOptions { BaseUrl = "https://bidkee.com" };
        Assert.Equal("https://bidkee.com/", o.GetBaseUri().ToString());
    }

    [Fact]
    public void Token_prefixes()
    {
        Assert.True(BidkeeToken.IsIssuer("iss_dev_local"));
        Assert.True(BidkeeToken.IsAgent("agt_abc"));
        Assert.False(BidkeeToken.IsIssuer("agt_abc"));
        Assert.Equal("BID-ABC123", BidkeeToken.NormalizePassNo("bid-abc123"));
    }

    [Fact]
    public void Exception_maps_status()
    {
        var ex = BidkeeException.FromStatus(System.Net.HttpStatusCode.Unauthorized, null, "no token", "send iss_");
        Assert.Equal("UNAUTHORIZED", ex.Code);
        Assert.True(ex.IsUnauthorized);
        Assert.Equal("send iss_", ex.Hint);
    }

    [Fact]
    public void Connect_exposes_public_and_agent()
    {
        using var g = BidkeeGateway.Connect(new BidkeeOptions { BaseUrl = "https://bidkee.com" });
        Assert.NotNull(g.Public);
        Assert.NotNull(g.Agents);
        Assert.Equal(BidkeeOptions.SupportedProtocol, "1.1");
    }
}
