using System;
using System.Collections.Specialized;
using System.Net;
using System.Web;
using MyAuth.OAuthPoint.Db;
using MyAuth.OAuthPoint.Services;
using MyAuth.OAuthPoint.Tools;
using MyLab.ApiClient.Test;

namespace FuncTests
{
    static class TestTools
    {
        public static string ClientPasswordHash { get; }

        public static string ClientPassword { get; }

        static TestTools()
        {
            var passCalculator = new PasswordHashCalculator("salt");

            ClientPassword = "password";
            ClientPasswordHash = passCalculator.CalcHexPasswordMd5(ClientPassword);
        }

        public static void TryExtractRedirect(
            TestCallDetails resp,
            out string locationLeftPart,
            out NameValueCollection query)
        {
            if (resp.StatusCode == HttpStatusCode.Redirect)
            {
                var newLocationUrl = resp.ResponseMessage.Headers.Location;

                if (newLocationUrl != null)
                {
                    query = HttpUtility.ParseQueryString(newLocationUrl.Query);
                    locationLeftPart = newLocationUrl.GetLeftPart(UriPartial.Path);

                    return;
                }
            }

            locationLeftPart = null;
            query = new NameValueCollection();
        }

        public static DataDbInitializer CreateDataIniterWithExpiredSession(string sessionId)
        {
            var clientId = Guid.NewGuid().ToString("N");
            var redirectUri = "http://host.net/cb";

            return new DataDbInitializer
            {
                Clients = new[] { new ClientDb { Id = clientId, Name = "foo", PasswordHash = TestTools.ClientPasswordHash } },
                LoginSessions = new[]
                {
                    new LoginSessionDb
                    {
                        Id = sessionId,
                        ClientId = clientId,
                        Expiry = DateTime.MinValue,
                        RedirectUri = redirectUri,
                        Scope = "no-mater-scope"
                    }
                }
            };
        }
    }
}