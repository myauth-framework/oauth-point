namespace MyAuth.OAuthPoint.Models
{
    public enum AuthorizationRequestProcessingError
    {
        Undefined,
        InvalidRequest,
        UnauthorizedClient,
        AccessDenied,
        UnsupportedResponseType,
        InvalidScope,
        ServerError,
        TempUnavailable
    }
}