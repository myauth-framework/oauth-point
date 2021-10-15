using System;
using Microsoft.AspNetCore.Http;

namespace MyAuth.OAuthPoint.Tools
{
    class LoginSessionCookie
    {
        public string SessionId { get; }

        public TimeSpan? Expiry { get; set; }

        public LoginSessionCookie(string sessionId)
        {
            SessionId = sessionId;
        }

        public void Save(HttpResponse response)
        {
            var opts = new CookieOptions();

            if (Expiry.HasValue)
            {
                opts.Expires = DateTimeOffset.Now.Add(Expiry.Value);
            }

            response.Cookies.Append(LoginSessionCookieName.Name, SessionId, opts);
        }

        public static bool TryLoad(HttpRequest request, out LoginSessionCookie cookie)
        {
            cookie = request.Cookies.TryGetValue(LoginSessionCookieName.Name, out var val)
                ? new LoginSessionCookie(val)
                : null;
            return cookie != null;
        }
    }
}
