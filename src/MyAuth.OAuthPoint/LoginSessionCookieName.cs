
#if MYAUTH_CLIENT
namespace MyAuth.OAuthPoint.Client
#else
namespace MyAuth.OAuthPoint
#endif
{
    /// <summary>
    /// Defines login session cookie name
    /// </summary>
    public static class LoginSessionCookieName
    {
        /// <summary>
        /// Name value
        /// </summary>
        public const string Name = "MYAUTH_LOGIN_SESSION_ID";
    }
}
