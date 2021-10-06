using System;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Mvc;
using MyAuth.OAuthPoint.Models;

namespace MyAuth.OAuthPoint.Tools
{
    static class UrlRedirector
    {
        public static IActionResult RedirectDefaultError(string defaultErrorEp, string errorDescription)
        {
            return new RedirectResult(new UriBuilder(defaultErrorEp){Query = "error_description=" + errorDescription}.ToString());
        }

        public static IActionResult RedirectCallbackError(string clientCallbackEp, AuthorizationRequestProcessingError error, string errorDescription, string state)
        {
            if (error == AuthorizationRequestProcessingError.Undefined)
                return null;

            var query = new NameValueCollection
            {
                {"error", EnumNameAttribute.GetName(error)}, 
                {"error_description", errorDescription}
            };

            if (state != null)
            {
                query.Add("state", state);
            }

            var url = new UriBuilder(clientCallbackEp) { Query = query.ToString() ?? "" };

            return new RedirectResult(url.ToString());
        }

        public static IActionResult RedirectToLogin(string loginEp, string loginId)
        {
            var url = new UriBuilder(loginEp) {Query = "login_id=" + loginId}.ToString();

            return new RedirectResult(url);
        }

        public static IActionResult RedirectSuccessCallback(string callbackEp, string authCode, string state)
        {
            var query = new NameValueCollection
            {
                {"code", authCode}
            };

            if (state != null)
            {
                query.Add("state", state);
            }

            var url = new UriBuilder(callbackEp) { Query = query.ToString() ?? "" };

            return new RedirectResult(url.ToString());
        }
    }
}
