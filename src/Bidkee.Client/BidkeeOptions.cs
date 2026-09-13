using System;
using System.Net.Http;

namespace Bidkee.Client
{
    /// <summary>Connection settings for bidkee.com or a self-hosted Plat.Api.</summary>
    public sealed class BidkeeOptions
    {
        public const string DefaultBaseUrl = "https://bidkee.com";
        public const string SupportedProtocol = "1.1";

        /// <summary>API origin. Trailing slash is normalized.</summary>
        public string BaseUrl { get; set; } = DefaultBaseUrl;

        /// <summary>Issuer token <c>iss_…</c> used to enroll agent instances.</summary>
        public string? IssuerToken { get; set; }

        /// <summary>Optional existing instance token <c>agt_…</c>.</summary>
        public string? AccessToken { get; set; }

        /// <summary>Ops key for Bidkee.Client.Ops only.</summary>
        public string? OpsApiKey { get; set; }

        public string? Tenant { get; set; }

        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>Re-enroll when the instance token is near expiry.</summary>
        public bool AutoRenew { get; set; } = true;

        public TimeSpan RenewSkew { get; set; } = TimeSpan.FromMinutes(5);

        public string UserAgent { get; set; } = "Bidkee.Client/1.1.0";

        /// <summary>Optional caller-owned handler. The gateway does not dispose it.</summary>
        public HttpMessageHandler? Handler { get; set; }

        public bool DisposeHandler { get; set; }

        public Uri GetBaseUri()
        {
            var raw = string.IsNullOrWhiteSpace(BaseUrl) ? DefaultBaseUrl : BaseUrl.Trim();
            if (!raw.EndsWith("/", StringComparison.Ordinal))
                raw += "/";
            return new Uri(raw, UriKind.Absolute);
        }
    }
}
