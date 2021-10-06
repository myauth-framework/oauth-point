#if MYAUTH_CLIENT
namespace MyAuth.OAuthPoint.Client.Models
#else
namespace MyAuth.OAuthPoint.Models 
#endif
{
    /// <summary>
    /// Login process type
    /// </summary>
    public enum LoginErrorType
    {
        /// <summary>
        /// Undefined
        /// </summary>
        Undefined,
        /// <summary>
        /// User cancel login
        /// </summary>
        UserCancel,
        /// <summary>
        /// Server reject user credentials
        /// </summary>
        ServerReject,
        /// <summary>
        /// Unhandled server error
        /// </summary>
        ServerError,
        /// <summary>
        /// Server can't process login temporarily
        /// </summary>
        TemporarilyUnavailable
    }
}