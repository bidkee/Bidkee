using Xunit;

namespace BidkeeClient.Tests;

public class CliSmokeTests
{
    [Fact]
    public void Assembly_is_named_BidkeeClient()
    {
        var name = typeof(Program).Assembly.GetName().Name;
        Assert.Equal("BidkeeClient", name);
    }
}
