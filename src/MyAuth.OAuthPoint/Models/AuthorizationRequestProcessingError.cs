#if MYAUTH_CLIENT
namespace MyAuth.OAuthPoint.Client.Models
#else

using LinqToDB.Mapping;
using MyAuth.OAuthPoint.Tools;

namespace MyAuth.OAuthPoint.Models
#endif
{
    /// <summary>
    /// Authorization process error
    /// </summary>
    public enum AuthorizationRequestProcessingError
    {
        /// <summary>
        /// Undefined value
        /// </summary>
#if !MYAUTH_CLIENT
        [MapValue(null)]
#endif
        Undefined,

        //OAuth2 https://datatracker.ietf.org/doc/html/rfc6749#section-4.1.2.1

        /// <summary>
        /// The request is missing a required parameter, includes an
        /// invalid parameter value, includes a parameter more than
        /// once, or is otherwise malformed.
        /// </summary>
#if !MYAUTH_CLIENT
        [EnumName("invalid_request")]
        [MapValue("invalid_request")]
#endif
        InvalidRequest,
        /// <summary>
        /// The client is not authorized to request an authorization
        /// code using this method.
        /// </summary>
#if !MYAUTH_CLIENT
        [EnumName("unauthorized_client")]
        [MapValue("unauthorized_client")]
#endif
        UnauthorizedClient,
        /// <summary>
        /// The resource owner or authorization server denied the
        /// request.
        /// </summary>
#if !MYAUTH_CLIENT
        [EnumName("access_denied")]
        [MapValue("access_denied")]
#endif
        AccessDenied,
        /// <summary>
        /// The authorization server does not support obtaining an
        /// authorization code using this method.
        /// </summary>
#if !MYAUTH_CLIENT
        [EnumName("unsupported_response_type")]
        [MapValue("unsupported_response_type")]
#endif
        UnsupportedResponseType,
        /// <summary>
        /// The requested scope is invalid, unknown, or malformed.
        /// </summary>
#if !MYAUTH_CLIENT
        [EnumName("invalid_scope")]
        [MapValue("invalid_scope")]
#endif
        InvalidScope,
        /// <summary>
        /// The authorization server encountered an unexpected
        /// condition that prevented it from fulfilling the request.
        /// (This error code is needed because a 500 Internal Server
        /// Error HTTP status code cannot be returned to the client
        /// via an HTTP redirect.)
        /// </summary>
#if !MYAUTH_CLIENT
        [EnumName("server_error")]
        [MapValue("server_error")]
#endif
        ServerError,
        /// <summary>
        /// The authorization server is currently unable to handle
        /// the request due to a temporary overloading or maintenance
        /// of the server.  (This error code is needed because a 503
        /// Service Unavailable HTTP status code cannot be returned
        /// to the client via an HTTP redirect.)
        /// </summary>
#if !MYAUTH_CLIENT
        [EnumName("temporarily_unavailable")]
        [MapValue("temporarily_unavailable")]
#endif
        TempUnavailable,

        //OpenID Connect https://openid.net/specs/openid-connect-core-1_0.html#AuthError

        /// <summary>
        /// The Authorization Server requires End-User interaction of some form to proceed. This error MAY be returned when the prompt parameter value in the Authentication Request is none, but the Authentication Request cannot be completed without displaying a user interface for End-User interaction.
        /// </summary>
#if !MYAUTH_CLIENT
        [EnumName("interaction_required")]
        [MapValue("interaction_required")]
#endif
        InteractionRequired,
        /// <summary>
        /// The Authorization Server requires End-User authentication. This error MAY be returned when the prompt parameter value in the Authentication Request is none, but the Authentication Request cannot be completed without displaying a user interface for End-User authentication.
        /// </summary>
#if !MYAUTH_CLIENT
        [EnumName("login_required")]
        [MapValue("login_required")]
#endif
        LoginRequired,
        /// <summary>
        /// The End-User is REQUIRED to select a session at the Authorization Server. The End-User MAY be authenticated at the Authorization Server with different associated accounts, but the End-User did not select a session. This error MAY be returned when the prompt parameter value in the Authentication Request is none, but the Authentication Request cannot be completed without displaying a user interface to prompt for a session to use.
        /// </summary>
#if !MYAUTH_CLIENT
        [EnumName("account_selection_required")]
        [MapValue("account_selection_required")]
#endif
        AccountSelectionRequired,
        /// <summary>
        /// The Authorization Server requires End-User consent. This error MAY be returned when the prompt parameter value in the Authentication Request is none, but the Authentication Request cannot be completed without displaying a user interface for End-User consent.
        /// </summary>
#if !MYAUTH_CLIENT
        [EnumName("consent_required")]
        [MapValue("consent_required")]
#endif
        ConsentRequired,
        /// <summary>
        /// The request_uri in the Authorization Request returns an error or contains invalid data.
        /// </summary>
#if !MYAUTH_CLIENT
        [EnumName("invalid_request_uri")]
        [MapValue("invalid_request_uri")]
#endif
        InvalidRequestUri,
        /// <summary>
        /// The request parameter contains an invalid Request Object.
        /// </summary>
#if !MYAUTH_CLIENT
        [EnumName("invalid_request_object")]
        [MapValue("invalid_request_object")]
#endif
        InvalidRequestObject,
        /// <summary>
        /// The OP does not support use of the request parameter defined in Section 6.
        /// </summary>
#if !MYAUTH_CLIENT
        [EnumName("request_not_supported")]
        [MapValue("request_not_supported")]
#endif
        RequestNotSupported,
        /// <summary>
        /// The OP does not support use of the request_uri parameter defined in Section 6.
        /// </summary>
#if !MYAUTH_CLIENT
        [EnumName("request_uri_not_supported")]
        [MapValue("request_uri_not_supported")]
#endif
        RequestUriNotSupported,
        /// <summary>
        /// The OP does not support use of the registration parameter defined in Section 7.2.1.
        /// </summary>
#if !MYAUTH_CLIENT
        [EnumName("registration_not_supported")]
        [MapValue("registration_not_supported")]
#endif
        RegistrationNotSupported
    }
}