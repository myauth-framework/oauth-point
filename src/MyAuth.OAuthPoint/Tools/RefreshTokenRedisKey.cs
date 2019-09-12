using Microsoft.AspNetCore.Mvc;

namespace MyAuth.OAuthPoint.Tools
{
    static class RefreshTokenRedisKey
    {
        public static string Create(string refreshToken)
        {
            return "myauth:refresh-token:" + refreshToken;
        }
    }

    static class SubjectRefreshTokensRedisKey
    {
        public static string Create(string subject)
        {
            return "myauth:subject-refresh-tokens:" + subject;
        }
    }
}