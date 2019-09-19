namespace MyAuth.OAuthPoint.Tools
{
    static class LoginRedisKey
    {
        public static string Create(string authCode)
        {
            return "myauth:auth-code:" + authCode;
        }
    }
}