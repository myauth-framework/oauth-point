using System.Threading.Tasks;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Models.DataContract;

namespace MyAuth.OAuthPoint.Services
{
    public interface ITokenService
    {
        Task<SuccessfulTokenResponse> IssueAsync(string clientId, string clientPassword, TokenRequest request);
    }
}
