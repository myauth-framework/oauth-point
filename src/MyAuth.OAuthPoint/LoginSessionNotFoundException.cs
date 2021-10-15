using System;
using MyLab.Log;

namespace MyAuth.OAuthPoint
{
    class LoginSessionNotFoundException : Exception
    {
        public LoginSessionNotFoundException() : base("Login session not found")
        {
        }
    }
}