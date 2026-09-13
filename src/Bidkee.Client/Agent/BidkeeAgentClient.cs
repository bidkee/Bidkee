using System;
using System.Threading;
using System.Threading.Tasks;
using Bidkee.Client.Auth;
using Bidkee.Client.Dtos;
using Bidkee.Client.Internal;

namespace Bidkee.Client.Agent
{
    public sealed class BidkeeAgentClient : IBidkeeAgentClient
    {
        readonly BidkeeHttp _http;
        readonly BidkeeOptions _options;

        internal BidkeeAgentClient(BidkeeHttp http, BidkeeOptions options)
        {
            _http = http;
            _options = options;
        }

        public Task<CapabilitiesResponse> GetCapabilitiesAsync(CancellationToken cancellationToken = default)
            => _http.GetAsync<CapabilitiesResponse>(HttpPaths.AgentCapabilities, cancellationToken);

        public async Task<BidkeeAgentSession> EnrollAsync(AgentEnrollRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.AgentId))
                throw new BidkeeException("MALFORMED", "agentId required", 400);
            if (string.IsNullOrWhiteSpace(request.InstanceId))
                request.InstanceId = Environment.MachineName;
            if (request.Purpose == null || request.Purpose.Length == 0)
                request.Purpose = new[] { "search", "verify", "present" };

            EnsureIssuer();
            var previous = _http.Bearer;
            _http.Bearer = _options.IssuerToken;
            try
            {
                var res = await _http.PostAsync<AgentEnrollResponse>(HttpPaths.AgentEnroll, request, cancellationToken).ConfigureAwait(false);
                if (!res.Ok)
                    throw new BidkeeException(res.Code, res.Detail ?? res.Code, 422, res.Detail);
                return new BidkeeAgentSession(_http, _options, res, request);
            }
            finally
            {
                _http.Bearer = previous;
            }
        }

        public async Task<BidkeeAgentSession> ResumeAsync(string accessToken, CancellationToken cancellationToken = default)
        {
            if (!BidkeeToken.IsAgent(accessToken))
                throw new BidkeeException("MALFORMED", "agt_ access token required", 400);
            var previous = _http.Bearer;
            _http.Bearer = accessToken.Trim();
            try
            {
                var me = await _http.GetAsync<AgentStatusResponse>(HttpPaths.AgentMe, cancellationToken).ConfigureAwait(false);
                if (!me.Ok)
                    throw new BidkeeException(me.Code, me.Code, 401);
                var enrolled = new AgentEnrollResponse
                {
                    Ok = true,
                    Code = "OK",
                    PassNo = me.PassNo,
                    CheckCode = me.CheckCode,
                    Status = me.Status,
                    ExpiresAt = me.ExpiresAt,
                    Issuer = me.Issuer,
                    AccessToken = accessToken.Trim(),
                    Capabilities = me.Capabilities
                };
                return new BidkeeAgentSession(_http, _options, enrolled, new AgentEnrollRequest
                {
                    AgentId = me.AgentId ?? "",
                    InstanceId = me.InstanceId ?? ""
                });
            }
            finally
            {
                _http.Bearer = previous;
            }
        }

        public async Task<BidkeeAgentSession> DelegateAsync(AgentDelegateRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.AgentId))
                throw new BidkeeException("MALFORMED", "agentId required", 400);
            EnsureIssuer();
            var previous = _http.Bearer;
            _http.Bearer = _options.IssuerToken;
            try
            {
                var res = await _http.PostAsync<AgentEnrollResponse>(HttpPaths.AgentDelegate, request, cancellationToken).ConfigureAwait(false);
                if (!res.Ok)
                    throw new BidkeeException(res.Code, res.Detail ?? res.Code, 422, res.Detail);
                return new BidkeeAgentSession(_http, _options, res, new AgentEnrollRequest
                {
                    AgentId = request.AgentId,
                    InstanceId = request.InstanceId,
                    TtlHours = request.TtlHours,
                    Purpose = request.Purpose
                });
            }
            finally
            {
                _http.Bearer = previous;
            }
        }

        void EnsureIssuer()
        {
            if (!BidkeeToken.IsIssuer(_options.IssuerToken))
                throw new BidkeeException("UNAUTHORIZED", "Set BidkeeOptions.IssuerToken to an iss_ token.", 401,
                    hint: "POST /v1/ops/agent/issuers with X-Bidkee-Ops-Key, then store the returned token.");
        }
    }
}
