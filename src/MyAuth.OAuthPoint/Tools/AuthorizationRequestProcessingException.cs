using System;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Mvc;
using MyAuth.OAuthPoint.Models;

namespace MyAuth.OAuthPoint.Tools
{
    class AuthorizationRequestProcessingException : Exception
    {
        public AuthorizationRequestProcessingError Reason { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="AuthorizationRequestProcessingException"/>
        /// </summary>
        public AuthorizationRequestProcessingException(string message, AuthorizationRequestProcessingError reason)
            :base(message)
        {
            Reason = reason;
        }
    }
}