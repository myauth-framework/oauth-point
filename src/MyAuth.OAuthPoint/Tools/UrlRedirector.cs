using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using MyAuth.OAuthPoint.Models;
using static System.String;

namespace MyAuth.OAuthPoint.Tools
{
    static class UrlRedirector
    {
        public static IActionResult RedirectError(string clientCallbackEp, AuthorizationRequestProcessingError error, string errorDescription, string state)
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

            var url = new UriBuilder(clientCallbackEp) { Query = NameValuesToQueryString(query) };

            return new RedirectResult(url.ToString(), false);
        }

        public static IActionResult RedirectToLogin(string loginEp, string loginId)
        {
            var url = new UriBuilder(loginEp) {Query = "login_id=" + loginId}.ToString();

            return new RedirectResult(url, false);
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

            var url = new UriBuilder(callbackEp) { Query = NameValuesToQueryString(query) };

            return new RedirectResult(url.ToString(), false);
        }

        static string NameValuesToQueryString(NameValueCollection c)
        {
            return Join("&",c.AllKeys.Select(a => a + "=" + HttpUtility.UrlEncode(c[a])));
        }
    }
}
