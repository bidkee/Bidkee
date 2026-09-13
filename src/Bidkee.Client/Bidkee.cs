using System;
using Bidkee.Client.Agent;
using Bidkee.Client.Grok;
using Bidkee.Client.Internal;
using Bidkee.Client.Public;

namespace Bidkee.Client
{
    /// <summary>Entry point for Bidkee.Client. Call <see cref="Connect"/>.</summary>
    public sealed class BidkeeGateway : IDisposable
    {
        readonly BidkeeHttp _http;

        public static BidkeeGateway Connect(BidkeeOptions? options = null)
            => new BidkeeGateway(options ?? new BidkeeOptions());

        internal BidkeeGateway(BidkeeOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            _http = new BidkeeHttp(options);
            Public = new BidkeePublicClient(_http);
            Agents = new BidkeeAgentClient(_http, options);
            Grok = new GrokBotClient(this);
        }

        public BidkeeOptions Options { get; }
        public IBidkeePublicClient Public { get; }
        public IBidkeeAgentClient Agents { get; }
        public GrokBotClient Grok { get; }

        public void Dispose() => _http.Dispose();
    }
}
