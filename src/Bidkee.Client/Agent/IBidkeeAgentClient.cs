using System.Threading;
using System.Threading.Tasks;
using Bidkee.Client.Dtos;

namespace Bidkee.Client.Agent
{
    public interface IBidkeeAgentClient
    {
        Task<CapabilitiesResponse> GetCapabilitiesAsync(CancellationToken cancellationToken = default);
        Task<BidkeeAgentSession> EnrollAsync(AgentEnrollRequest request, CancellationToken cancellationToken = default);
        Task<BidkeeAgentSession> ResumeAsync(string accessToken, CancellationToken cancellationToken = default);
        Task<BidkeeAgentSession> DelegateAsync(AgentDelegateRequest request, CancellationToken cancellationToken = default);
    }
}
