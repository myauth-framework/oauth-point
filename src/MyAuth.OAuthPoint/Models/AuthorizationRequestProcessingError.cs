using MyAuth.OAuthPoint.Tools;

namespace MyAuth.OAuthPoint.Models
{
    public enum AuthorizationRequestProcessingError
    {
        Undefined,

        //OAuth2 https://datatracker.ietf.org/doc/html/rfc6749#section-4.1.2.1

        /// <summary>
        /// The request is missing a required parameter, includes an
        /// invalid parameter value, includes a parameter more than
        /// once, or is otherwise malformed.
        /// </summary>
        [EnumName("invalid_request")]
        InvalidRequest,
        /// <summary>
        /// The client is not authorized to request an authorization
        /// code using this method.
        /// </summary>
        [EnumName("unauthorized_client")]
        UnauthorizedClient,
        /// <summary>
        /// The resource owner or authorization server denied the
        /// request.
        /// </summary>
        [EnumName("access_denied")]
        AccessDenied,
        /// <summary>
        /// The authorization server does not support obtaining an
        /// authorization code using this method.
        /// </summary>
        [EnumName("unsupported_response_type")]
        UnsupportedResponseType,
        /// <summary>
        /// The requested scope is invalid, unknown, or malformed.
        /// </summary>
        [EnumName("invalid_scope")]
        InvalidScope,
        /// <summary>
        /// The authorization server encountered an unexpected
        /// condition that prevented it from fulfilling the request.
        /// (This error code is needed because a 500 Internal Server
        /// Error HTTP status code cannot be returned to the client
        /// via an HTTP redirect.)
        /// </summary>
        [EnumName("server_error")]
        ServerError,
        /// <summary>
        /// The authorization server is currently unable to handle
        /// the request due to a temporary overloading or maintenance
        /// of the server.  (This error code is needed because a 503
        /// Service Unavailable HTTP status code cannot be returned
        /// to the client via an HTTP redirect.)
        /// </summary>
        [EnumName("temporarily_unavailable")]
        TempUnavailable,

        //OpenID Connect https://openid.net/specs/openid-connect-core-1_0.html#AuthError

        /// <summary>
        /// The Authorization Server requires End-User interaction of some form to proceed. This error MAY be returned when the prompt parameter value in the Authentication Request is none, but the Authentication Request cannot be completed without displaying a user interface for End-User interaction.
        /// </summary>
        [EnumName("interaction_required")]
        InteractionRequired,
        /// <summary>
        /// The Authorization Server requires End-User authentication. This error MAY be returned when the prompt parameter value in the Authentication Request is none, but the Authentication Request cannot be completed without displaying a user interface for End-User authentication.
        /// </summary>
        [EnumName("login_required")]
        LoginRequired,
        /// <summary>
        /// The End-User is REQUIRED to select a session at the Authorization Server. The End-User MAY be authenticated at the Authorization Server with different associated accounts, but the End-User did not select a session. This error MAY be returned when the prompt parameter value in the Authentication Request is none, but the Authentication Request cannot be completed without displaying a user interface to prompt for a session to use.
        /// </summary>
        [EnumName("account_selection_required")]
        AccountSelectionRequired,
        /// <summary>
        /// The Authorization Server requires End-User consent. This error MAY be returned when the prompt parameter value in the Authentication Request is none, but the Authentication Request cannot be completed without displaying a user interface for End-User consent.
        /// </summary>
        [EnumName("consent_required")]
        ConsentRequired,
        /// <summary>
        /// The request_uri in the Authorization Request returns an error or contains invalid data.
        /// </summary>
        [EnumName("invalid_request_uri")]
        InvalidRequestUri,
        /// <summary>
        /// The request parameter contains an invalid Request Object.
        /// </summary>
        [EnumName("invalid_request_object")]
        InvalidRequestObject,
        /// <summary>
        /// The OP does not support use of the request parameter defined in Section 6.
        /// </summary>
        [EnumName("request_not_supported")]
        RequestNotSupported,
        /// <summary>
        /// The OP does not support use of the request_uri parameter defined in Section 6.
        /// </summary>
        [EnumName("request_uri_not_supported")]
        RequestUriNotSupported,
        /// <summary>
        /// The OP does not support use of the registration parameter defined in Section 7.2.1.
        /// </summary>
        [EnumName("registration_not_supported")]
        RegistrationNotSupported
    }
}