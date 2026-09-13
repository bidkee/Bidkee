using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Bidkee.Client.Auth;
using Bidkee.Client.Dtos;

namespace Bidkee.Client.Internal
{
    internal sealed class BidkeeHttp : IDisposable
    {
        readonly bool _ownClient;
        public HttpClient Client { get; }
        public BidkeeOptions Options { get; }

        public string? Bearer { get; set; }
        public string? PassNo { get; set; }

        public BidkeeHttp(BidkeeOptions options, HttpClient? client = null)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            if (client != null)
            {
                Client = client;
                _ownClient = false;
            }
            else
            {
                Client = options.Handler != null
                    ? new HttpClient(options.Handler, options.DisposeHandler)
                    : new HttpClient();
                _ownClient = true;
            }
            Client.BaseAddress = options.GetBaseUri();
            Client.Timeout = options.Timeout;
            if (!string.IsNullOrWhiteSpace(options.UserAgent))
            {
                Client.DefaultRequestHeaders.UserAgent.Clear();
                Client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", options.UserAgent);
            }
            Bearer = options.AccessToken ?? options.IssuerToken;
        }

        public async Task<T> GetAsync<T>(string path, CancellationToken ct)
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, path);
            Apply(req);
            using var res = await Client.SendAsync(req, ct).ConfigureAwait(false);
            return await ReadAsync<T>(res, ct).ConfigureAwait(false);
        }

        public async Task<T> PostAsync<T>(string path, object? body, CancellationToken ct)
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, path);
            Apply(req);
            if (body != null)
                req.Content = JsonContent.Create(body, options: BidkeeJson.Options);
            using var res = await Client.SendAsync(req, ct).ConfigureAwait(false);
            return await ReadAsync<T>(res, ct).ConfigureAwait(false);
        }

        public async Task<DownloadedTicket> GetBytesAsync(string path, CancellationToken ct)
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, path);
            Apply(req);
            using var res = await Client.SendAsync(req, ct).ConfigureAwait(false);
            if (!res.IsSuccessStatusCode)
            {
                var err = await TryReadError(res, ct).ConfigureAwait(false);
                throw BidkeeException.FromStatus(res.StatusCode, err?.Code ?? err?.Error, err?.Detail ?? err?.Message, err?.Hint);
            }
            var bytes = await res.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var name = res.Content.Headers.ContentDisposition?.FileNameStar
                       ?? res.Content.Headers.ContentDisposition?.FileName
                       ?? "download";
            if (name.Length >= 2 && name[0] == '"')
                name = name.Trim('"');
            return new DownloadedTicket
            {
                Bytes = bytes,
                ContentType = res.Content.Headers.ContentType?.MediaType ?? "application/octet-stream",
                FileName = name
            };
        }

        public async Task<string> GetStringAsync(string path, CancellationToken ct)
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, path);
            Apply(req);
            using var res = await Client.SendAsync(req, ct).ConfigureAwait(false);
            var text = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!res.IsSuccessStatusCode)
            {
                var err = TryParseError(text);
                throw BidkeeException.FromStatus(res.StatusCode, err?.Code ?? err?.Error, err?.Detail ?? err?.Message ?? text, err?.Hint);
            }
            return text;
        }

        void Apply(HttpRequestMessage req)
        {
            if (!string.IsNullOrWhiteSpace(Bearer))
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Bearer);
            if (!string.IsNullOrWhiteSpace(Options.Tenant))
                req.Headers.TryAddWithoutValidation(BidkeeToken.TenantHeader, Options.Tenant);
            if (!string.IsNullOrWhiteSpace(PassNo))
                req.Headers.TryAddWithoutValidation(BidkeeToken.PassHeader, PassNo);
            if (BidkeeToken.IsIssuer(Bearer))
                req.Headers.TryAddWithoutValidation(BidkeeToken.EnrollHeader, Bearer);
            if (!string.IsNullOrWhiteSpace(Options.OpsApiKey))
                req.Headers.TryAddWithoutValidation(BidkeeToken.OpsHeader, Options.OpsApiKey);
        }

        static async Task<T> ReadAsync<T>(HttpResponseMessage res, CancellationToken ct)
        {
            var text = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!res.IsSuccessStatusCode)
            {
                var err = TryParseError(text);
                var ex = BidkeeException.FromStatus(res.StatusCode, err?.Code ?? err?.Error, err?.Detail ?? err?.Message ?? Trim(text), err?.Hint);
                if (res.Headers.RetryAfter?.Delta != null)
                    ex.RetryAfter = res.Headers.RetryAfter.Delta;
                throw ex;
            }
            if (string.IsNullOrWhiteSpace(text))
                return default!;
            try
            {
                var value = JsonSerializer.Deserialize<T>(text, BidkeeJson.Options);
                if (value == null)
                    throw new BidkeeException("MALFORMED", "Empty JSON body.", (int)res.StatusCode);
                return value;
            }
            catch (JsonException ex)
            {
                throw new BidkeeException("MALFORMED", "JSON parse failed: " + ex.Message, (int)res.StatusCode, text);
            }
        }

        static async Task<ErrorBody?> TryReadError(HttpResponseMessage res, CancellationToken ct)
        {
            var text = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
            return TryParseError(text);
        }

        static ErrorBody? TryParseError(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            try { return JsonSerializer.Deserialize<ErrorBody>(text, BidkeeJson.Options); }
            catch { return new ErrorBody { Detail = Trim(text) }; }
        }

        static string Trim(string s)
        {
            if (s == null) return "";
            s = s.Trim();
            return s.Length <= 400 ? s : s.Substring(0, 400);
        }

        public void Dispose()
        {
            if (_ownClient)
                Client.Dispose();
        }
    }
}
