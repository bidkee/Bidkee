using System;
using System.Threading;
using System.Threading.Tasks;
using Bidkee.Client.Auth;
using Bidkee.Client.Dtos;
using Bidkee.Client.Internal;
using Bidkee.Client.Public;

namespace Bidkee.Client.Agent
{
    /// <summary>Live agent instance bound to an <c>agt_</c> token and <c>BID-…</c> pass.</summary>
    public sealed class BidkeeAgentSession : IDisposable
    {
        readonly BidkeeHttp _http;
        readonly BidkeeOptions _options;
        readonly AgentEnrollRequest _origin;
        readonly BidkeePublicClient _public;

        internal BidkeeAgentSession(BidkeeHttp http, BidkeeOptions options, AgentEnrollResponse enrolled, AgentEnrollRequest origin)
        {
            _http = http;
            _options = options;
            _origin = origin;
            AccessToken = enrolled.AccessToken ?? options.AccessToken ?? "";
            PassNo = BidkeeToken.NormalizePassNo(enrolled.PassNo);
            CheckCode = enrolled.CheckCode;
            ExpiresAt = enrolled.ExpiresAt;
            Capabilities = enrolled.Capabilities;
            Status = enrolled.Status;
            Issuer = enrolled.Issuer ?? enrolled.IssuerName;
            Ticket = enrolled.Ticket;
            _public = new BidkeePublicClient(http);
        }

        public string AccessToken { get; private set; }
        public string PassNo { get; private set; }
        public string? CheckCode { get; private set; }
        public DateTimeOffset? ExpiresAt { get; private set; }
        public string[]? Capabilities { get; private set; }
        public string? Status { get; private set; }
        public string? Issuer { get; private set; }
        public System.Text.Json.JsonElement? Ticket { get; private set; }
        public string AgentId => _origin.AgentId;
        public string InstanceId => _origin.InstanceId;

        public bool IsExpired
        {
            get
            {
                if (ExpiresAt == null) return false;
                return DateTimeOffset.UtcNow + _options.RenewSkew >= ExpiresAt.Value;
            }
        }

        public Task<SearchResponse> SearchAsync(string? q, CancellationToken cancellationToken = default)
            => WithAgent(ct => _public.SearchAsync(q, ct), cancellationToken);

        public Task<TicketDetail> GetAsync(string checkCode, CancellationToken cancellationToken = default)
            => WithAgent(ct => _public.GetAsync(checkCode, ct), cancellationToken);

        public Task<DownloadedTicket> DownloadAsync(string checkCode, string format = "json", CancellationToken cancellationToken = default)
            => WithAgent(ct => _public.DownloadAsync(checkCode, format, ct), cancellationToken);

        public Task<object> ResolveAsync(string id, CancellationToken cancellationToken = default)
            => WithAgent(ct => _public.ResolveAsync(id, ct), cancellationToken);

        public Task<AgentPresentStartResponse> PresentStartAsync(CancellationToken cancellationToken = default)
            => WithAgent(ct => _http.PostAsync<AgentPresentStartResponse>(HttpPaths.AgentPresentStart, new { }, ct), cancellationToken);

        public Task<AgentGateResponse> PresentCompleteAsync(string challenge, CancellationToken cancellationToken = default)
            => WithAgent(ct => _http.PostAsync<AgentGateResponse>(HttpPaths.AgentPresentComplete, new AgentPresentCompleteRequest
            {
                Challenge = challenge,
                PassNo = PassNo
            }, ct), cancellationToken);

        public Task<VerifyResponse> VerifyDirectoryAsync(string checkCode, CancellationToken cancellationToken = default)
            => WithAgent(ct => _public.VerifyAsync(new VerifyRequest { CheckCode = checkCode }, ct), cancellationToken);

        public Task<OfflineVerifyResponse> VerifyOfflineAsync(string ticketJson, CancellationToken cancellationToken = default)
            => WithAgent(ct => _public.VerifyOfflineAsync(ticketJson, ct), cancellationToken);

        public Task<AgentStatusResponse> MeAsync(CancellationToken cancellationToken = default)
            => WithAgent(async ct =>
            {
                var me = await _http.GetAsync<AgentStatusResponse>(HttpPaths.AgentMe, ct).ConfigureAwait(false);
                if (me.Ok)
                {
                    PassNo = BidkeeToken.NormalizePassNo(me.PassNo);
                    CheckCode = me.CheckCode;
                    ExpiresAt = me.ExpiresAt;
                    Status = me.Status;
                    Capabilities = me.Capabilities;
                }
                return me;
            }, cancellationToken);

        public Task<AgentGateResponse> VerifyPassAsync(string? passNo = null, string? checkCode = null, string? json = null, CancellationToken cancellationToken = default)
            => WithAgent(ct => _http.PostAsync<AgentGateResponse>(HttpPaths.AgentVerify, new AgentVerifyRequest
            {
                PassNo = BidkeeToken.NormalizePassNo(passNo ?? PassNo),
                CheckCode = checkCode ?? CheckCode,
                Json = json
            }, ct), cancellationToken);

        /// <summary>Platform-proxied present. Agents do not holder-sign.</summary>
        public Task<AgentGateResponse> PresentAsync(CancellationToken cancellationToken = default)
            => WithAgent(async ct =>
            {
                var start = await _http.PostAsync<AgentPresentStartResponse>(HttpPaths.AgentPresentStart, new { }, ct).ConfigureAwait(false);
                if (start == null || string.IsNullOrWhiteSpace(start.Challenge))
                    throw new BidkeeException("MALFORMED", "present/start returned no challenge", 422);
                return await _http.PostAsync<AgentGateResponse>(HttpPaths.AgentPresentComplete, new AgentPresentCompleteRequest
                {
                    Challenge = start.Challenge,
                    PassNo = PassNo
                }, ct).ConfigureAwait(false);
            }, cancellationToken);

        public Task<RevokePassResponse> RevokeAsync(CancellationToken cancellationToken = default)
            => WithAgent(ct => _http.PostAsync<RevokePassResponse>(HttpPaths.AgentRevoke, new AgentPresentCompleteRequest { PassNo = PassNo }, ct), cancellationToken);

        async Task<T> WithAgent<T>(Func<CancellationToken, Task<T>> action, CancellationToken ct)
        {
            await EnsureFreshAsync(ct).ConfigureAwait(false);
            var previous = _http.Bearer;
            var previousPass = _http.PassNo;
            _http.Bearer = AccessToken;
            _http.PassNo = PassNo;
            try
            {
                return await action(ct).ConfigureAwait(false);
            }
            finally
            {
                _http.Bearer = previous;
                _http.PassNo = previousPass;
            }
        }

        async Task EnsureFreshAsync(CancellationToken ct)
        {
            if (!_options.AutoRenew || !IsExpired)
                return;
            if (!BidkeeToken.IsIssuer(_options.IssuerToken))
                return;
            var previous = _http.Bearer;
            _http.Bearer = _options.IssuerToken;
            try
            {
                var res = await _http.PostAsync<AgentEnrollResponse>(HttpPaths.AgentEnroll, _origin, ct).ConfigureAwait(false);
                if (!res.Ok || string.IsNullOrWhiteSpace(res.AccessToken))
                    return;
                AccessToken = res.AccessToken!;
                PassNo = BidkeeToken.NormalizePassNo(res.PassNo);
                CheckCode = res.CheckCode;
                ExpiresAt = res.ExpiresAt;
                Capabilities = res.Capabilities;
                Status = res.Status;
            }
            finally
            {
                _http.Bearer = previous;
            }
        }

        public void Dispose()
        {
            // Session does not own the shared HttpClient.
        }
    }
}
