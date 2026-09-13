using System;
using System.Text.Json;

namespace Bidkee.Client.Dtos
{
    public sealed class AgentEnrollRequest
    {
        public string AgentId { get; set; } = "";
        public string InstanceId { get; set; } = "";
        public int? TtlHours { get; set; }
        public string[]? Purpose { get; set; }
        public bool IncludeTicket { get; set; }
    }

    public sealed class AgentEnrollResponse
    {
        public bool Ok { get; set; }
        public string Code { get; set; } = "";
        public string? Detail { get; set; }
        public string? PassNo { get; set; }
        public string? CheckCode { get; set; }
        public string? Status { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public string? Issuer { get; set; }
        public string? IssuerName { get; set; }
        public string? AccessToken { get; set; }
        public string[]? Capabilities { get; set; }
        public JsonElement? Ticket { get; set; }
        public string? PresentUrl { get; set; }
    }

    public sealed class AgentStatusResponse
    {
        public bool Ok { get; set; }
        public string Code { get; set; } = "";
        public string? PassNo { get; set; }
        public string? CheckCode { get; set; }
        public string? Status { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public string? Issuer { get; set; }
        public string? AgentId { get; set; }
        public string? InstanceId { get; set; }
        public string[]? Capabilities { get; set; }
    }

    public sealed class AgentPresentStartResponse
    {
        public bool Ok { get; set; }
        public string? Challenge { get; set; }
        public string? PassNo { get; set; }
        public DateTimeOffset? ChallengeExpiresAt { get; set; }
    }

    public sealed class AgentPresentCompleteRequest
    {
        public string? Challenge { get; set; }
        public string? PassNo { get; set; }
    }

    public sealed class AgentGateResponse
    {
        public bool Allowed { get; set; }
        public string Code { get; set; } = "";
        public string? Detail { get; set; }
        public string? PassNo { get; set; }
        public string? Status { get; set; }
        public string? Issuer { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
    }

    public sealed class AgentVerifyRequest
    {
        public string? PassNo { get; set; }
        public string? CheckCode { get; set; }
        public string? Json { get; set; }
    }

    public sealed class AgentDelegateRequest
    {
        public string? ParentPassNo { get; set; }
        public string? ParentCheckCode { get; set; }
        public string AgentId { get; set; } = "";
        public string InstanceId { get; set; } = "";
        public int? TtlHours { get; set; }
        public string[]? Purpose { get; set; }
        public bool IncludeTicket { get; set; }
    }
}
