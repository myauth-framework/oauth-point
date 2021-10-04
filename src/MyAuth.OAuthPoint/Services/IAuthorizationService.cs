using System;
using MyAuth.OAuthPoint.Models;

namespace MyAuth.OAuthPoint.Services
{
    public interface IAuthorizationService
    {
        string CreateLoginSession();

        LoginSassion GetLoginSession(string loginSessionId);
    }
}
