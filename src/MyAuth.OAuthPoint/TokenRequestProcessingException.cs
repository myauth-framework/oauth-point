using System;
using MyAuth.OAuthPoint.Models;

namespace MyAuth.OAuthPoint
{
    class TokenRequestProcessingException : Exception
    {
        public TokenRequestProcessingError Reason { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="TokenRequestProcessingException"/>
        /// </summary>
        public TokenRequestProcessingException(string message, TokenRequestProcessingError reason)
            : base(message)
        {
            Reason = reason;
        }
    }
}