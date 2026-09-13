using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Bidkee.Client.Dtos;
using Bidkee.Client.Internal;

namespace Bidkee.Client.Public
{
    public sealed class BidkeePublicClient : IBidkeePublicClient
    {
        readonly BidkeeHttp _http;

        internal BidkeePublicClient(BidkeeHttp http)
        {
            _http = http;
        }

        public Task<HealthResponse> HealthAsync(CancellationToken cancellationToken = default)
            => _http.GetAsync<HealthResponse>(HttpPaths.Health, cancellationToken);

        public Task<object> GatewayAsync(CancellationToken cancellationToken = default)
            => _http.GetAsync<object>(HttpPaths.Gateway, cancellationToken);

        public Task<SearchResponse> SearchAsync(string? q, CancellationToken cancellationToken = default)
        {
            var path = HttpPaths.Search + "?q=" + Uri.EscapeDataString(q ?? "");
            return _http.GetAsync<SearchResponse>(path, cancellationToken);
        }

        public Task<TicketDetail> GetAsync(string checkCode, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(checkCode))
                throw new BidkeeException("MALFORMED", "checkCode required", 400);
            return _http.GetAsync<TicketDetail>(HttpPaths.Id + Uri.EscapeDataString(checkCode.Trim()), cancellationToken);
        }

        public Task<DownloadedTicket> DownloadAsync(string checkCode, string format = "json", CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(checkCode))
                throw new BidkeeException("MALFORMED", "checkCode required", 400);
            var fmt = string.IsNullOrWhiteSpace(format) ? "json" : format.Trim();
            var path = HttpPaths.Id + Uri.EscapeDataString(checkCode.Trim()) + "/download?fmt=" + Uri.EscapeDataString(fmt);
            return _http.GetBytesAsync(path, cancellationToken);
        }

        public async Task<object> ResolveAsync(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new BidkeeException("MALFORMED", "id required", 400);
            var json = await _http.GetStringAsync(HttpPaths.Resolve + Uri.EscapeDataString(id.Trim()), cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<JsonElement>(json, BidkeeJson.Options);
        }

        public Task<VerifyResponse> VerifyAsync(VerifyRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.CheckCode))
                throw new BidkeeException("MALFORMED", "checkCode required", 400);
            return _http.PostAsync<VerifyResponse>(HttpPaths.Verify, request, cancellationToken);
        }

        public Task<OfflineVerifyResponse> VerifyOfflineAsync(string ticketJson, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(ticketJson))
                throw new BidkeeException("MALFORMED", "ticket JSON required", 400);
            return _http.PostAsync<OfflineVerifyResponse>(HttpPaths.VerifyOffline, new OfflineVerifyRequest { Json = ticketJson }, cancellationToken);
        }

        public Task<PresentStartResponse> StartPresentAsync(CancellationToken cancellationToken = default)
            => _http.PostAsync<PresentStartResponse>(HttpPaths.PresentStart, new { }, cancellationToken);

        public Task<PresentCompleteResponse> CompletePresentAsync(PresentCompleteRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new BidkeeException("MALFORMED", "present body required", 400);
            return _http.PostAsync<PresentCompleteResponse>(HttpPaths.PresentComplete, request, cancellationToken);
        }

        public Task<UploadResult> UploadAsync(string ticketJson, string? fileName = null, CancellationToken cancellationToken = default)
            => UploadAsync(new UploadRequest { Json = ticketJson, FileName = fileName, Source = "paste" }, cancellationToken);

        public Task<UploadResult> UploadAsync(UploadRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Json))
                throw new BidkeeException("MALFORMED", "ticket JSON required", 400);
            if (string.IsNullOrWhiteSpace(request.Source))
                request.Source = "paste";
            return _http.PostAsync<UploadResult>(HttpPaths.Ingest, request, cancellationToken);
        }
    }
}
