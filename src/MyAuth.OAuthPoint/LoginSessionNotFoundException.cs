using System;
using MyLab.Log;

namespace MyAuth.OAuthPoint
{
    class LoginSessionNotFoundException : Exception
    {
        public LoginSessionNotFoundException(string loginSessionId) : base("Login session not found")
        {
            this.AndFactIs("login-session-id", loginSessionId);
        }
    }
}