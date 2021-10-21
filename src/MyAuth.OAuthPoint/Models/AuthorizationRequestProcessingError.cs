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
    /// Authorization request processing error
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
        [EnumName("invalid_request")]
#if !MYAUTH_CLIENT
        [MapValue("invalid_request")]
#endif
        InvalidRequest,
        /// <summary>
        /// The client is not authorized to request an authorization
        /// code using this method.
        /// </summary>
#if !MYAUTH_CLIENT
        [MapValue("unauthorized_client")]
#endif
        [EnumName("unauthorized_client")]
        UnauthorizedClient,
        /// <summary>
        /// The resource owner or authorization server denied the
        /// request.
        /// </summary>
#if !MYAUTH_CLIENT
        [MapValue("access_denied")]
#endif
        [EnumName("access_denied")]
        AccessDenied,
        /// <summary>
        /// The authorization server does not support obtaining an
        /// authorization code using this method.
        /// </summary>
#if !MYAUTH_CLIENT
        [MapValue("unsupported_response_type")]
#endif
        [EnumName("unsupported_response_type")]
        UnsupportedResponseType,
        /// <summary>
        /// The requested scope is invalid, unknown, or malformed.
        /// </summary>
#if !MYAUTH_CLIENT
        [MapValue("invalid_scope")]
#endif
        [EnumName("invalid_scope")]
        InvalidScope,
        /// <summary>
        /// The authorization server encountered an unexpected
        /// condition that prevented it from fulfilling the request.
        /// (This error code is needed because a 500 Internal Server
        /// Error HTTP status code cannot be returned to the client
        /// via an HTTP redirect.)
        /// </summary>
#if !MYAUTH_CLIENT
        [MapValue("server_error")]
#endif
        [EnumName("server_error")]
        ServerError,
        /// <summary>
        /// The authorization server is currently unable to handle
        /// the request due to a temporary overloading or maintenance
        /// of the server.  (This error code is needed because a 503
        /// Service Unavailable HTTP status code cannot be returned
        /// to the client via an HTTP redirect.)
        /// </summary>
#if !MYAUTH_CLIENT
        [MapValue("temporarily_unavailable")]
#endif
        [EnumName("temporarily_unavailable")]
        TempUnavailable,

        //OpenID Connect https://openid.net/specs/openid-connect-core-1_0.html#AuthError

        /// <summary>
        /// The Authorization Server requires End-User interaction of some form to proceed. This error MAY be returned when the prompt parameter value in the Authentication Request is none, but the Authentication Request cannot be completed without displaying a user interface for End-User interaction.
        /// </summary>
#if !MYAUTH_CLIENT
        [MapValue("interaction_required")]
#endif
        [EnumName("interaction_required")]
        InteractionRequired,
        /// <summary>
        /// The Authorization Server requires End-User authentication. This error MAY be returned when the prompt parameter value in the Authentication Request is none, but the Authentication Request cannot be completed without displaying a user interface for End-User authentication.
        /// </summary>
#if !MYAUTH_CLIENT
        [MapValue("login_required")]
#endif
        [EnumName("login_required")]
        LoginRequired,
        /// <summary>
        /// The End-User is REQUIRED to select a session at the Authorization Server. The End-User MAY be authenticated at the Authorization Server with different associated accounts, but the End-User did not select a session. This error MAY be returned when the prompt parameter value in the Authentication Request is none, but the Authentication Request cannot be completed without displaying a user interface to prompt for a session to use.
        /// </summary>
#if !MYAUTH_CLIENT
        [MapValue("account_selection_required")]
#endif
        [EnumName("account_selection_required")]
        AccountSelectionRequired,
        /// <summary>
        /// The Authorization Server requires End-User consent. This error MAY be returned when the prompt parameter value in the Authentication Request is none, but the Authentication Request cannot be completed without displaying a user interface for End-User consent.
        /// </summary>
#if !MYAUTH_CLIENT
        [MapValue("consent_required")]
#endif
        [EnumName("consent_required")]
        ConsentRequired,
        /// <summary>
        /// The request_uri in the Authorization Request returns an error or contains invalid data.
        /// </summary>
#if !MYAUTH_CLIENT
        [MapValue("invalid_request_uri")]
#endif
        [EnumName("invalid_request_uri")]
        InvalidRequestUri,
        /// <summary>
        /// The request parameter contains an invalid Request Object.
        /// </summary>
#if !MYAUTH_CLIENT
        [MapValue("invalid_request_object")]
#endif
        [EnumName("invalid_request_object")]
        InvalidRequestObject,
        /// <summary>
        /// The OP does not support use of the request parameter defined in Section 6.
        /// </summary>
#if !MYAUTH_CLIENT
        [MapValue("request_not_supported")]
#endif
        [EnumName("request_not_supported")]
        RequestNotSupported,
        /// <summary>
        /// The OP does not support use of the request_uri parameter defined in Section 6.
        /// </summary>
#if !MYAUTH_CLIENT
        [MapValue("request_uri_not_supported")]
#endif
        [EnumName("request_uri_not_supported")]
        RequestUriNotSupported,
        /// <summary>
        /// The OP does not support use of the registration parameter defined in Section 7.2.1.
        /// </summary>
#if !MYAUTH_CLIENT
        [MapValue("registration_not_supported")]
#endif
        [EnumName("registration_not_supported")]
        RegistrationNotSupported
    }
}