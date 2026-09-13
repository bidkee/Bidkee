using System.Threading;
using System.Threading.Tasks;
using Bidkee.Client.Dtos;

namespace Bidkee.Client.Public
{
    public interface IBidkeePublicClient
    {
        Task<HealthResponse> HealthAsync(CancellationToken cancellationToken = default);
        Task<object> GatewayAsync(CancellationToken cancellationToken = default);
        Task<SearchResponse> SearchAsync(string? q, CancellationToken cancellationToken = default);
        Task<TicketDetail> GetAsync(string checkCode, CancellationToken cancellationToken = default);
        Task<DownloadedTicket> DownloadAsync(string checkCode, string format = "json", CancellationToken cancellationToken = default);
        Task<object> ResolveAsync(string id, CancellationToken cancellationToken = default);
        Task<VerifyResponse> VerifyAsync(VerifyRequest request, CancellationToken cancellationToken = default);
        Task<OfflineVerifyResponse> VerifyOfflineAsync(string ticketJson, CancellationToken cancellationToken = default);
        Task<PresentStartResponse> StartPresentAsync(CancellationToken cancellationToken = default);
        Task<PresentCompleteResponse> CompletePresentAsync(PresentCompleteRequest request, CancellationToken cancellationToken = default);
        Task<UploadResult> UploadAsync(string ticketJson, string? fileName = null, CancellationToken cancellationToken = default);
        Task<UploadResult> UploadAsync(UploadRequest request, CancellationToken cancellationToken = default);
    }
}
