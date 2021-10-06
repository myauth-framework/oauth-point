using MyAuth.OAuthPoint.Models;

namespace MyAuth.OAuthPoint.Services
{
    class LoginService : ILoginService
    {
        public void CreateLoginSession(out string loginSessionId)
        {
            throw new System.NotImplementedException();
        }

        public LoginSession GetLoginSession(string loginSessionId)
        {
            throw new System.NotImplementedException();
        }

        public void SaveSuccess(string loginSessionId, AuthorizedUserInfo authorizedUserInfo)
        {
            throw new System.NotImplementedException();
        }

        public void SaveError(string loginSessionId, LoginError loginError)
        {
            throw new System.NotImplementedException();
        }
    }
}