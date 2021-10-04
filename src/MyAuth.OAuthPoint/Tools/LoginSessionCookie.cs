using System;
using Microsoft.AspNetCore.Http;

namespace MyAuth.OAuthPoint.Tools
{
    public class LoginSessionCookie
    {
        const string CookieName = "MyAuth:LoginSessionId";

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

            response.Cookies.Append(CookieName, SessionId, opts);
        }

        public static bool TryLoad(HttpRequest request, out LoginSessionCookie cookie)
        {
            cookie = request.Cookies.TryGetValue(CookieName, out var val)
                ? new LoginSessionCookie(val)
                : null;
            return cookie != null;
        }
    }
}
