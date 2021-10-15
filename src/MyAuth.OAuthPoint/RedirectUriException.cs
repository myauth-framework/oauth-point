using System;

namespace MyAuth.OAuthPoint
{
    class RedirectUriException : Exception
    {
        public string WrongUri { get; }

        public RedirectUriException(string wrongUri, string message) : base(message)
        {
            WrongUri = wrongUri;
        }
    }
}
