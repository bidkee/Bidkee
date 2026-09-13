using System.Text.Json;
using Bidkee.Client;
using Bidkee.Client.Dtos;
using Xunit;

namespace Bidkee.Client.Tests;

public class JsonContractTests
{
    [Fact]
    public void EnrollRequest_serializes_camelCase()
    {
        var json = JsonSerializer.Serialize(new AgentEnrollRequest
        {
            AgentId = "grok-bot",
            InstanceId = "dev-1",
            TtlHours = 24,
            Purpose = ["search", "verify", "present"],
            IncludeTicket = false
        }, BidkeeJson.Options);

        Assert.Contains("\"agentId\":\"grok-bot\"", json);
        Assert.Contains("\"instanceId\":\"dev-1\"", json);
        Assert.Contains("\"ttlHours\":24", json);
        Assert.DoesNotContain("\"AgentId\"", json);
    }

    [Fact]
    public void EnrollResponse_reads_server_shape()
    {
        const string json = """
            {"ok":true,"code":"OK","passNo":"BID-ABC123","checkCode":"ab","accessToken":"agt_x","capabilities":["search"]}
            """;
        var r = JsonSerializer.Deserialize<AgentEnrollResponse>(json, BidkeeJson.Options);
        Assert.NotNull(r);
        Assert.True(r!.Ok);
        Assert.Equal("BID-ABC123", r.PassNo);
        Assert.Equal("agt_x", r.AccessToken);
    }

    [Fact]
    public void SearchHit_reads_public_directory()
    {
        const string json = """
            {"q":"kaspa:q","hits":[{"checkCode":"aa","did":"did:bidkee:kaspa:x","status":"active","sponsored":false,"queryCount":3}]}
            """;
        var r = JsonSerializer.Deserialize<SearchResponse>(json, BidkeeJson.Options);
        Assert.Single(r!.Hits);
        Assert.Equal("aa", r.Hits[0].CheckCode);
        Assert.Equal(3, r.Hits[0].QueryCount);
    }

    [Fact]
    public void UploadRequest_serializes_ingest_body()
    {
        var json = JsonSerializer.Serialize(new UploadRequest
        {
            Json = "{\"checkCode\":\"aa\"}",
            FileName = "ticket.json",
            Source = "paste"
        }, BidkeeJson.Options);
        Assert.Contains("\"fileName\":\"ticket.json\"", json);
        Assert.Contains("\"source\":\"paste\"", json);
    }
}
