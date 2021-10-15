using System;

namespace MyAuth.OAuthPoint
{
    class LoginSessionExpiredException : Exception
    {
        public LoginSessionExpiredException() : base("Login session expired")
        {
        }
    }
}