using System;
using System.Threading;
using System.Threading.Tasks;
using Bidkee.Client.Agent;
using Bidkee.Client.Auth;
using Bidkee.Client.Dtos;
using Bidkee.Client.Internal;
using Bidkee.Client.Public;

namespace Bidkee.Client.Grok
{
    /// <summary>
    /// All routes a Grok Bot is allowed to call on BidkeePlat.
    /// Does not wrap issue / claim / ingest (those need a human holder signature or Ops).
    /// </summary>
    public sealed class GrokBotClient : IDisposable
    {
        readonly BidkeeGateway _gateway;
        readonly bool _ownGateway;
        BidkeeAgentSession? _session;

        public GrokBotClient(BidkeeOptions options)
        {
            _gateway = BidkeeGateway.Connect(options);
            _ownGateway = true;
        }

        public GrokBotClient(BidkeeGateway gateway)
        {
            _gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
            _ownGateway = false;
        }

        public BidkeeGateway Gateway => _gateway;
        public BidkeeAgentSession? Session => _session;
        public IBidkeePublicClient Public => _gateway.Public;
        public IBidkeeAgentClient Agents => _gateway.Agents;

        public Task<HealthResponse> HealthAsync(CancellationToken ct = default)
            => _gateway.Public.HealthAsync(ct);

        public Task<CapabilitiesResponse> CapabilitiesAsync(CancellationToken ct = default)
            => _gateway.Agents.GetCapabilitiesAsync(ct);

        public Task<object> GatewayCatalogAsync(CancellationToken ct = default)
            => _gateway.Public.GatewayAsync(ct);

        public async Task<BidkeeAgentSession> EnrollAsync(
            string? agentId = null,
            string? instanceId = null,
            int ttlHours = 24,
            string[]? purpose = null,
            bool includeTicket = false,
            CancellationToken ct = default)
        {
            var req = new AgentEnrollRequest
            {
                AgentId = string.IsNullOrWhiteSpace(agentId) ? GrokBotTools.DefaultAgentId : agentId!,
                InstanceId = string.IsNullOrWhiteSpace(instanceId) ? Environment.MachineName : instanceId!,
                TtlHours = ttlHours,
                Purpose = purpose == null || purpose.Length == 0 ? GrokBotTools.DefaultPurposes : purpose,
                IncludeTicket = includeTicket
            };
            _session = await _gateway.Agents.EnrollAsync(req, ct).ConfigureAwait(false);
            return _session;
        }

        public async Task<BidkeeAgentSession> ResumeAsync(string accessToken, CancellationToken ct = default)
        {
            _session = await _gateway.Agents.ResumeAsync(accessToken, ct).ConfigureAwait(false);
            return _session;
        }

        public async Task<BidkeeAgentSession> AttachAsync(CancellationToken ct = default)
        {
            if (_session != null) return _session;
            if (BidkeeToken.IsAgent(_gateway.Options.AccessToken))
                return await ResumeAsync(_gateway.Options.AccessToken!, ct).ConfigureAwait(false);
            if (BidkeeToken.IsIssuer(_gateway.Options.IssuerToken))
                return await EnrollAsync(ct: ct).ConfigureAwait(false);
            throw new BidkeeException("UNAUTHORIZED",
                "Grok Bot needs BIDKEE_ISSUER_TOKEN (iss_) to enroll or BIDKEE_ACCESS_TOKEN (agt_) to resume.",
                401);
        }

        public Task<AgentStatusResponse> MeAsync(CancellationToken ct = default)
            => Require().MeAsync(ct);

        public Task<SearchResponse> SearchAsync(string? q, CancellationToken ct = default)
            => PublicOrAgent(s => s.SearchAsync(q, ct), () => _gateway.Public.SearchAsync(q, ct), ct);

        public Task<TicketDetail> LookupAsync(string checkCode, CancellationToken ct = default)
            => PublicOrAgent(s => s.GetAsync(checkCode, ct), () => _gateway.Public.GetAsync(checkCode, ct), ct);

        public Task<DownloadedTicket> DownloadAsync(string checkCode, string format = "json", CancellationToken ct = default)
            => PublicOrAgent(s => s.DownloadAsync(checkCode, format, ct), () => _gateway.Public.DownloadAsync(checkCode, format, ct), ct);

        public Task<object> ResolveAsync(string id, CancellationToken ct = default)
            => PublicOrAgent(s => s.ResolveAsync(id, ct), () => _gateway.Public.ResolveAsync(id, ct), ct);

        public Task<VerifyResponse> VerifyAsync(string checkCode, decimal? amount = null, CancellationToken ct = default)
            => PublicOrAgent(
                s => s.VerifyDirectoryAsync(checkCode, ct),
                () => _gateway.Public.VerifyAsync(new VerifyRequest { CheckCode = checkCode, RequestedOrderAmount = amount }, ct),
                ct);

        public Task<OfflineVerifyResponse> VerifyOfflineAsync(string ticketJson, CancellationToken ct = default)
            => PublicOrAgent(s => s.VerifyOfflineAsync(ticketJson, ct), () => _gateway.Public.VerifyOfflineAsync(ticketJson, ct), ct);

        public Task<AgentGateResponse> VerifyPassAsync(string? passNo = null, string? checkCode = null, string? json = null, CancellationToken ct = default)
            => Require().VerifyPassAsync(passNo, checkCode, json, ct);

        public Task<AgentGateResponse> PresentAsync(CancellationToken ct = default)
            => Require().PresentAsync(ct);

        public Task<AgentPresentStartResponse> PresentStartAsync(CancellationToken ct = default)
            => Require().PresentStartAsync(ct);

        public Task<AgentGateResponse> PresentCompleteAsync(string challenge, CancellationToken ct = default)
            => Require().PresentCompleteAsync(challenge, ct);

        public async Task<BidkeeAgentSession> DelegateAsync(
            string parentPassNo,
            string? agentId = null,
            string? instanceId = null,
            int ttlHours = 4,
            string[]? purpose = null,
            CancellationToken ct = default)
        {
            _session = await _gateway.Agents.DelegateAsync(new AgentDelegateRequest
            {
                ParentPassNo = BidkeeToken.NormalizePassNo(parentPassNo),
                AgentId = string.IsNullOrWhiteSpace(agentId) ? GrokBotTools.DefaultAgentId : agentId!,
                InstanceId = string.IsNullOrWhiteSpace(instanceId) ? Environment.MachineName + "-delegated" : instanceId!,
                TtlHours = ttlHours,
                Purpose = purpose == null || purpose.Length == 0 ? GrokBotTools.DefaultPurposes : purpose
            }, ct).ConfigureAwait(false);
            return _session;
        }

        public Task<RevokePassResponse> RevokeAsync(CancellationToken ct = default)
            => Require().RevokeAsync(ct);

        BidkeeAgentSession Require()
        {
            if (_session != null) return _session;
            throw new BidkeeException("UNAUTHORIZED", "Call EnrollAsync / ResumeAsync / AttachAsync first.", 401);
        }

        async Task<T> PublicOrAgent<T>(Func<BidkeeAgentSession, Task<T>> agent, Func<Task<T>> anon, CancellationToken ct)
        {
            if (_session != null)
                return await agent(_session).ConfigureAwait(false);
            try
            {
                if (BidkeeToken.IsAgent(_gateway.Options.AccessToken) || BidkeeToken.IsIssuer(_gateway.Options.IssuerToken))
                {
                    await AttachAsync(ct).ConfigureAwait(false);
                    return await agent(_session!).ConfigureAwait(false);
                }
            }
            catch (BidkeeException)
            {
                // fall back to anonymous public routes
            }
            return await anon().ConfigureAwait(false);
        }

        public void Dispose()
        {
            if (_ownGateway) _gateway.Dispose();
        }
    }
}
