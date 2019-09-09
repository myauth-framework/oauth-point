using System.Linq;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Services;

namespace MyAuth.OAuthPoint.Tools
{
    class LoginRequestChecker
    {
        private readonly IClientRegistry _clientRegistry;

        public LoginRequestChecker(IClientRegistry clientRegistry)
        {
            _clientRegistry = clientRegistry;
        }

        public bool Check(LoginRequest loginRequest, out string errText)
        {
            if (loginRequest == null)
            {
                errText = "Request is empty";
                return false;
            }

            if (string.IsNullOrWhiteSpace(loginRequest.ClientId))
            {
                errText =  "ClientId is required";
                return false;
            }

            if (string.IsNullOrWhiteSpace(loginRequest.RedirectUri))
            {
                errText =  "RedirectUri is required";
                return false;
            }

            if (string.IsNullOrWhiteSpace(loginRequest.UserId))
            {
                errText =  "UserId is required";
                return false;
            }

            if (!string.IsNullOrEmpty(loginRequest.CodeChallenge) &&
                loginRequest.CodeChallengeMethod.ToLower() != "md5")
            {
                errText =  "Code challenge method not supported";
                return false;
            }

            var client = _clientRegistry.GetClient(loginRequest.ClientId);
            if (client == null)
            {
                errText =  "Client not found";
                return false;
            }

            if (client.Verification && string.IsNullOrEmpty(loginRequest.CodeChallenge))
            {
                errText =  "Code challenge is required for this client";
                return false;
            }

            if (!CheckUri(loginRequest.RedirectUri, client))
            {
                errText =  "Wrong redirect uri";
                return false;
            }
            
            bool CheckUri(string uri, ClientEntry c)
            {
                return c.AllowUris == null ||
                       c.AllowUris.Length == 0 ||
                       c.AllowUris.Any(u => u.ToLower() == uri.ToLower());
            }

            errText = null;
            return true;
        }
    }
}