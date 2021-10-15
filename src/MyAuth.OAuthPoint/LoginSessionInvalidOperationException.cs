using System;
using MyLab.Log;

namespace MyAuth.OAuthPoint
{
    class LoginSessionInvalidOperationException : Exception
    {
        public LoginSessionInvalidOperationException() : base("Can't perform operation due to login session state")
        {
            
        }
    }
}