#if MYAUTH_CLIENT

using MyAuth.OAuthPoint.Client.Tools;

namespace MyAuth.OAuthPoint.Client.Models
#else

using LinqToDB.Mapping;
using MyAuth.OAuthPoint.Tools;

namespace MyAuth.OAuthPoint.Models
#endif
{
    /// <summary>
    /// Token request processing error
    /// </summary>
    public enum TokenRequestProcessingError
    {
        /// <summary>
        /// Undefined value
        /// </summary>
        Undefined,

        //OAuth2 https://datatracker.ietf.org/doc/html/rfc6749#section-5.2

        /// <summary>
        /// The request is missing a required parameter, includes an
        /// invalid parameter value, includes a parameter more than
        /// once, or is otherwise malformed.
        /// </summary>
        [EnumName("invalid_request")]
        InvalidRequest,

        /// <summary>
        /// Client authentication failed (e.g., unknown client, no
        /// client authentication included, or unsupported
        /// authentication method).  The authorization server MAY
        /// return an HTTP 401 (Unauthorized) status code to indicate
        /// which HTTP authentication schemes are supported.  If the
        /// client attempted to authenticate via the "Authorization"
        /// request header field, the authorization server MUST
        /// respond with an HTTP 401 (Unauthorized) status code and
        /// include the "WWW-Authenticate" response header field
        /// matching the authentication scheme used by the client.
        /// </summary>
        [EnumName("invalid_client")]
        InvalidClient,

        /// <summary>
        /// The provided authorization grant (e.g., authorization
        /// code, resource owner credentials) or refresh token is
        /// invalid, expired, revoked, does not match the redirection
        /// URI used in the authorization request, or was issued to
        /// another client.
        /// </summary>
        [EnumName("invalid_grant")]
        InvalidGrant,

        /// <summary>
        /// The client is not authorized to request an authorization
        /// code using this method.
        /// </summary>
        [EnumName("unauthorized_client")]
        UnauthorizedClient,

        /// <summary>
        /// The authorization grant type is not supported by the
        /// authorization server.
        /// </summary>
        [EnumName("unsupported_grant_type")]
        UnsupportedGrantType,
        
        /// <summary>
        /// The requested scope is invalid, unknown, or malformed.
        /// </summary>
        [EnumName("invalid_scope")]
        InvalidScope
    }
}
