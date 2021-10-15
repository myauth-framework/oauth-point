using System.Threading.Tasks;
using MyAuth.OAuthPoint.Models;

namespace MyAuth.OAuthPoint.Services
{
    public interface ILoginService
    {
        Task<string> CreateLoginSessionAsync(AuthorizationRequest authorizationRequest);

        Task<LoginSession> GetLoginSessionAsync(string loginSessionId);
        Task<LoginSession> GetActiveLoginSessionAsync(string loginSessionId);

        Task SaveSuccessAsync(string loginSessionId, AuthorizedSubjectInfo authorizedSubjectInfo);

        Task SaveErrorAsync(string loginSessionId, LoginError loginError);
    }
}
 