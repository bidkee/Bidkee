using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Bidkee.Client.Dtos
{
    public sealed class SearchResponse
    {
        public string? Q { get; set; }
        public IList<SearchHit> Hits { get; set; } = new List<SearchHit>();
    }

    public sealed class SearchHit
    {
        public string? CheckCode { get; set; }
        public string? Title { get; set; }
        public string? Did { get; set; }
        public string? Status { get; set; }
        public string? DocumentKind { get; set; }
        public string? DeviceNumber { get; set; }
        public string? SuperAddress { get; set; }
        public string? FirstAddress { get; set; }
        public string? AuthorizingValidTo { get; set; }
        public int QueryCount { get; set; }
        public bool Sponsored { get; set; }
    }

    public sealed class TicketDetail
    {
        public string? CheckCode { get; set; }
        public string? Did { get; set; }
        public string? Status { get; set; }
        public string? SuperAddress { get; set; }
        public string? FirstAddress { get; set; }
        public string? DeviceNumber { get; set; }
        public string? ValidDate { get; set; }
        public string? AuthorizingValidTo { get; set; }
        public string? CustodyUntil { get; set; }
        public string? DocumentKind { get; set; }
        public bool CryptoValid { get; set; }
        public string? CryptoFailure { get; set; }
        public IList<string> FileHashes { get; set; } = new List<string>();
        public IList<string> Path { get; set; } = new List<string>();
        public JsonElement? Ticket { get; set; }
        public JsonElement? DidDocument { get; set; }
    }

    public sealed class VerifyRequest
    {
        public string? CheckCode { get; set; }
        public decimal? RequestedOrderAmount { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }

    public sealed class VerifyResponse
    {
        public bool Ok { get; set; }
        public string? Code { get; set; }
        public string? Detail { get; set; }
        public string? CheckCode { get; set; }
        public string? SuggestedOperation { get; set; }
        public decimal? RemainingCredit { get; set; }
        public string? CertNo { get; set; }
    }

    public sealed class OfflineVerifyRequest
    {
        public string? Json { get; set; }
    }

    public sealed class OfflineVerifyResponse
    {
        public bool Ok { get; set; }
        public string? Code { get; set; }
        public string? Detail { get; set; }
        public string? CheckCode { get; set; }
        public string? HolderAddress { get; set; }
        public string? GrantorAddress { get; set; }
        public string? DeviceNumber { get; set; }
        public bool Persisted { get; set; }
        public string? SuggestedOperation { get; set; }
    }

    public sealed class PresentStartResponse
    {
        public bool Ok { get; set; }
        public string? Challenge { get; set; }
    }

    public sealed class PresentCompleteRequest
    {
        public string? Challenge { get; set; }
        public string? CheckCode { get; set; }
        public string? HolderSignatureHex { get; set; }
    }

    public sealed class PresentCompleteResponse
    {
        public bool Ok { get; set; }
        public string? Code { get; set; }
        public string? Did { get; set; }
        public string? Detail { get; set; }
    }

    public sealed class DownloadedTicket
    {
        public byte[] Bytes { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = "application/json";
        public string FileName { get; set; } = "ticket.json";
    }

    public sealed class UploadRequest
    {
        public string? Json { get; set; }
        public string? FileName { get; set; }
        public string? Source { get; set; } = "paste";
    }

    public sealed class UploadResult
    {
        public bool Ok { get; set; }
        public string Code { get; set; } = "";
        public string? CheckCode { get; set; }
        public string? Did { get; set; }
        public string? DocumentKind { get; set; }
        public string? FailureReason { get; set; }
        public System.Collections.Generic.IList<string>? Warnings { get; set; }
    }
}
