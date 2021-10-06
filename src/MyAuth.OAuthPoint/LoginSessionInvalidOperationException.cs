using System;
using MyLab.Log;

namespace MyAuth.OAuthPoint
{
    class LoginSessionInvalidOperationException : Exception
    {
        public LoginSessionInvalidOperationException(string loginSessionId) : base("Can't perform operation due to login session state")
        {
            this.AndFactIs("login-session-id", loginSessionId);
        }
    }
}