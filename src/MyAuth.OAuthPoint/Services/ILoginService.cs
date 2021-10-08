using System.Threading.Tasks;
using MyAuth.OAuthPoint.Models;

namespace MyAuth.OAuthPoint.Services
{
    public interface ILoginService
    {
        Task<string> CreateLoginSessionAsync(AuthorizationRequest authorizationRequest);

        Task<LoginSession> GetLoginSessionAsync(string loginSessionId);

        Task SaveSuccessAsync(string loginSessionId, AuthorizedUserInfo authorizedUserInfo);

        Task SaveErrorAsync(string loginSessionId, LoginError loginError);
    }
}
 