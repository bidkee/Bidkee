using System.Collections.Generic;

namespace Bidkee.Client.Grok
{
    /// <summary>Tool names Grok Bot should register. Matches Plat agent + public directory.</summary>
    public static class GrokBotTools
    {
        public const string DefaultAgentId = "grok-bot";

        public static readonly string[] DefaultPurposes = { "search", "verify", "present" };

        public static IReadOnlyList<GrokToolDescriptor> All { get; } = new[]
        {
            T("bidkee_health", "GET /v1/health", "Probe protocol, HSM, DB, advertised capabilities."),
            T("bidkee_capabilities", "GET /v1/agent/capabilities", "Confirm agent_enroll, present_proxy, delegate, pass_no, holder-sign=false."),
            T("bidkee_gateway", "GET /v1/gateway", "Machine-readable catalog of every BidkeePlat route."),
            T("bidkee_enroll", "POST /v1/agent/enroll", "Mint agt_ token + BID- pass for this Grok Bot instance. Requires iss_."),
            T("bidkee_me", "GET /v1/agent/me", "Current instance pass, expiry, purposes."),
            T("bidkee_search", "GET /v1/search", "Look up address / DID / check code / file hash."),
            T("bidkee_lookup", "GET /v1/id/{check}", "Ticket detail. Hidden tickets are redacted."),
            T("bidkee_download", "GET /v1/id/{check}/download", "Download ticket as json, b64, or did."),
            T("bidkee_resolve", "GET /v1/resolve/{id}", "Raw ticket by check code."),
            T("bidkee_verify", "POST /v1/verify", "Directory verify (dual-sign + policy). May consume credit."),
            T("bidkee_verify_offline", "POST /v1/verify/offline", "Verify pasted JSON. No ingest, no debit."),
            T("bidkee_verify_pass", "POST /v1/agent/verify", "Verify this instance pass or another BID- pass."),
            T("bidkee_present", "POST /v1/agent/present/start+complete", "Platform-proxied present. Grok Bot does not holder-sign."),
            T("bidkee_present_start", "POST /v1/agent/present/start", "Get a present challenge."),
            T("bidkee_present_complete", "POST /v1/agent/present/complete", "Complete present with challenge."),
            T("bidkee_delegate", "POST /v1/agent/delegate", "Attenuate a user parent pass onto a child Grok Bot instance."),
            T("bidkee_revoke", "POST /v1/agent/revoke", "Revoke this instance pass.")
        };

        static GrokToolDescriptor T(string name, string route, string summary)
            => new GrokToolDescriptor { Name = name, Route = route, Summary = summary };
    }

    public sealed class GrokToolDescriptor
    {
        public string Name { get; set; } = "";
        public string Route { get; set; } = "";
        public string Summary { get; set; } = "";
    }
}
