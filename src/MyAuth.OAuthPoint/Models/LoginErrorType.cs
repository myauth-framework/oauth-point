namespace MyAuth.OAuthPoint.Models
{
    public enum LoginErrorType
    {
        Undefined,
        UserCancel,
        ServerReject,
        ServerError,
        TemporarilyUnavailable
    }
}