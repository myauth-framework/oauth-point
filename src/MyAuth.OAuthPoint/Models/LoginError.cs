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
        /// OpenID Connect authorization error code
        /// </summary>
        public AuthorizationRequestProcessingError AuthError { get; set; }
        /// <summary>
        /// Human readable description
        /// </summary>
        public string Description { get; set; }
    }
}