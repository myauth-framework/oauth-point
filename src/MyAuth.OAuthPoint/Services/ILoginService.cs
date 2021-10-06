using MyAuth.OAuthPoint.Models;

namespace MyAuth.OAuthPoint.Services
{
    public interface ILoginService
    {
        void CreateLoginSession(out string loginSessionId);

        LoginSession GetLoginSession(string loginSessionId);

        void SaveSuccess(string loginSessionId, AuthorizedUserInfo authorizedUserInfo);

        void SaveError(string loginSessionId, LoginError loginError);
    }
}
