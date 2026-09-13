using System;

namespace Bidkee.Client.Auth
{
    public static class BidkeeToken
    {
        public const string IssuerPrefix = "iss_";
        public const string AgentPrefix = "agt_";
        public const string OpsHeader = "X-Bidkee-Ops-Key";
        public const string SessionHeader = "X-Bidkee-Session";
        public const string TenantHeader = "X-Bidkee-Tenant";
        public const string EnrollHeader = "X-Bidkee-Enroll-Token";
        public const string PassHeader = "X-Bidkee-Pass";

        public static bool IsIssuer(string? token)
            => !string.IsNullOrWhiteSpace(token) &&
               token!.StartsWith(IssuerPrefix, StringComparison.OrdinalIgnoreCase);

        public static bool IsAgent(string? token)
            => !string.IsNullOrWhiteSpace(token) &&
               token!.StartsWith(AgentPrefix, StringComparison.OrdinalIgnoreCase);

        public static string NormalizePassNo(string? passNo)
            => string.IsNullOrWhiteSpace(passNo) ? "" : passNo!.Trim().ToUpperInvariant();
    }
}
