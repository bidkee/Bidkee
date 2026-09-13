using System;
using System.Threading;
using System.Threading.Tasks;
using Bidkee.Client;
using Bidkee.Client.Internal;

namespace Bidkee.Client.Ops
{
    public sealed class BidkeeOpsClient : IDisposable
    {
        readonly BidkeeHttp _http;
        readonly bool _own;

        public BidkeeOpsClient(BidkeeOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            _http = new BidkeeHttp(options);
            _own = true;
        }

        public BidkeeOpsClient(BidkeeGateway gateway)
        {
            if (gateway == null) throw new ArgumentNullException(nameof(gateway));
            _http = new BidkeeHttp(gateway.Options);
            _own = true;
        }

        public Task<IngestResult> IngestAsync(IngestRequest request, CancellationToken cancellationToken = default)
            => _http.PostAsync<IngestResult>(HttpPaths.Ingest, request, cancellationToken);

        public Task<ClaimResult> ClaimAsync(string password, CancellationToken cancellationToken = default)
            => _http.PostAsync<ClaimResult>(HttpPaths.Claim, new ClaimRequest { Password = password }, cancellationToken);

        public Task<ClaimResult> LoginAsync(string address, string password, CancellationToken cancellationToken = default)
            => _http.PostAsync<ClaimResult>(HttpPaths.ClaimLogin, new ClaimRequest { Address = address, Password = password }, cancellationToken);

        public Task<IssuePlanResult> PlanAsync(IssuePlanRequest request, CancellationToken cancellationToken = default)
            => _http.PostAsync<IssuePlanResult>(HttpPaths.IssuePlan, request, cancellationToken);

        public Task<IssueAuthorizeResult> AuthorizeAsync(string draftId, CancellationToken cancellationToken = default)
            => _http.PostAsync<IssueAuthorizeResult>(HttpPaths.IssueAuthorize, new IssueAuthorizeRequest { DraftId = draftId }, cancellationToken);

        public Task<IssueHolderCompleteResult> HolderCompleteAsync(IssueHolderCompleteRequest request, CancellationToken cancellationToken = default)
            => _http.PostAsync<IssueHolderCompleteResult>(HttpPaths.IssueHolderComplete, request, cancellationToken);

        public Task<object> AuthorizersAsync(CancellationToken cancellationToken = default)
            => _http.GetAsync<object>(HttpPaths.IssueAuthorizers, cancellationToken);

        public Task<object> SceneCatalogAsync(CancellationToken cancellationToken = default)
            => _http.GetAsync<object>(HttpPaths.SceneCatalog, cancellationToken);

        public Task<object> RevokeAsync(RevokeRequest request, CancellationToken cancellationToken = default)
            => _http.PostAsync<object>(HttpPaths.Revoke, request, cancellationToken);

        public Task<object> BindFileAsync(BindFileRequest request, CancellationToken cancellationToken = default)
            => _http.PostAsync<object>(HttpPaths.FilesBind, request, cancellationToken);

        public Task<object> VerifyFileAsync(BindFileRequest request, CancellationToken cancellationToken = default)
            => _http.PostAsync<object>(HttpPaths.FilesVerify, request, cancellationToken);

        public Task<AgentIssuerCreateResponse> CreateIssuerAsync(AgentIssuerCreateRequest request, CancellationToken cancellationToken = default)
            => _http.PostAsync<AgentIssuerCreateResponse>(HttpPaths.OpsAgentIssuers, request, cancellationToken);

        public void Dispose()
        {
            if (_own) _http.Dispose();
        }
    }
}
