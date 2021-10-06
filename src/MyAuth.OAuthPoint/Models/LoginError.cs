#if MYAUTH_CLIENT
namespace MyAuth.OAuthPoint.Client.Models
#else
namespace MyAuth.OAuthPoint.Models 
#endif
{
    /// <summary>
    /// Describes login process error
    /// </summary>
    public class LoginError
    {
        /// <summary>
        /// Error type
        /// </summary>
        public LoginErrorType Type { get; set; }
        /// <summary>
        /// Human readable description
        /// </summary>
        public string Description { get; set; }
    }
}