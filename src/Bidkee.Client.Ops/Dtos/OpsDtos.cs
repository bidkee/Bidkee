using System;
using System.Collections.Generic;

namespace Bidkee.Client.Ops
{
    public sealed class IngestRequest
    {
        public string? Json { get; set; }
        public string? FileName { get; set; }
        public string? Source { get; set; } = "paste";
    }

    public sealed class IngestResult
    {
        public bool Ok { get; set; }
        public string Code { get; set; } = "";
        public string? CheckCode { get; set; }
        public string? Did { get; set; }
        public string? DocumentKind { get; set; }
        public string? FailureReason { get; set; }
        public IList<string>? Warnings { get; set; }
    }

    public sealed class ClaimRequest
    {
        public string? Password { get; set; }
        public string? Address { get; set; }
    }

    public sealed class ClaimResult
    {
        public bool Ok { get; set; }
        public string Code { get; set; } = "";
        public string? FirstAddress { get; set; }
        public int? AccountIndex { get; set; }
        public string? CheckCode { get; set; }
        public string? Did { get; set; }
        public bool AlreadyIssued { get; set; }
        public string? FailureReason { get; set; }
        public string? SessionToken { get; set; }
        public string? Address { get; set; }
    }

    public sealed class IssuePlanRequest
    {
        public string Scene { get; set; } = "presentable_id";
        public string HashAlg { get; set; } = "sha256-canonical-v1.1";
        public string DeviceSigAlg { get; set; } = "sha256-canonical-v1.1";
        public string FirstAddress { get; set; } = "";
        public string SuperAddress { get; set; } = "";
        public string? DeviceNumber { get; set; }
        public decimal? CreditLimit { get; set; }
        public string? PresetAccount { get; set; }
        public string? ContentCommit { get; set; }
        public string? CommunicationAddress { get; set; }
        public string? DocumentKind { get; set; } = "ticket";
        public string Disclosure { get; set; } = "hint";
        public int ValidDays { get; set; } = 30;
        public int CustodyDays { get; set; } = 365;
        public bool CanDelegate { get; set; }
        public int MaxDepth { get; set; }
        public string? ParentCheckCode { get; set; }
        public bool SkipLlmReview { get; set; } = true;
    }

    public sealed class IssuePlanResult
    {
        public bool Ok { get; set; }
        public string Code { get; set; } = "";
        public string? DraftId { get; set; }
        public string? Status { get; set; }
        public string? AssociationCanonical { get; set; }
        public string? HashAlg { get; set; }
        public string? DeviceSigAlg { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public string? FailureReason { get; set; }
    }

    public sealed class IssueAuthorizeRequest
    {
        public string DraftId { get; set; } = "";
    }

    public sealed class IssueAuthorizeResult
    {
        public bool Ok { get; set; }
        public string Code { get; set; } = "";
        public string? DraftId { get; set; }
        public string? CheckCode { get; set; }
        public string? DeviceMessage { get; set; }
        public string? SuperSigHex { get; set; }
        public string? Status { get; set; }
        public string? FailureReason { get; set; }
    }

    public sealed class IssueHolderCompleteRequest
    {
        public string DraftId { get; set; } = "";
        public string FirstAddress { get; set; } = "";
        public string HolderSignature { get; set; } = "";
        public string? KaswarePossessionSignatureHex { get; set; }
        public string? KaswareChallengeId { get; set; }
    }

    public sealed class IssueHolderCompleteResult
    {
        public bool Ok { get; set; }
        public string Code { get; set; } = "";
        public string? CheckCode { get; set; }
        public string? Did { get; set; }
        public string? Status { get; set; }
        public string? FailureReason { get; set; }
    }

    public sealed class RevokeRequest
    {
        public string CheckCode { get; set; } = "";
        public string Reason { get; set; } = "";
        public string? SignatureHex { get; set; }
        public string? SignerAddress { get; set; }
    }

    public sealed class BindFileRequest
    {
        public string CheckCode { get; set; } = "";
        public string FileHash2 { get; set; } = "";
        public string? AttachType { get; set; } = "content";
    }

    public sealed class AgentIssuerCreateRequest
    {
        public string Name { get; set; } = "";
        public int? DailyQuota { get; set; }
        public int? MaxTtlHours { get; set; }
        public string[]? Purpose { get; set; }
        public string? Token { get; set; }
    }

    public sealed class AgentIssuerCreateResponse
    {
        public bool Ok { get; set; }
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Token { get; set; }
    }
}
