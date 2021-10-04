using Microsoft.Extensions.Localization;
using MyAuth.OAuthPoint.Models;

namespace MyAuth.OAuthPoint.Tools
{
    public class AuthorizationRequestValidator
    {
        private readonly IStringLocalizer<AuthorizationRequestValidator> _localizer;
        public AuthorizationRequestValidator(IStringLocalizer<AuthorizationRequestValidator> localizer)
        {
            _localizer = localizer;
        }

        public void Validate(AuthorizationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ResponseType))
                throw new AuthorizationRequestProcessingException(_localizer["RequiredParameterNotSpecified", "response_type"], AuthorizationRequestProcessingError.InvalidRequest);
            if(request.ResponseType != "code")
                throw new AuthorizationRequestProcessingException(_localizer["UnsupportedResponseType", request.ResponseType], AuthorizationRequestProcessingError.UnsupportedResponseType);

            if (string.IsNullOrWhiteSpace(request.ClientId))
                throw new AuthorizationRequestProcessingException(_localizer["RequiredParameterNotSpecified", "client_id"], AuthorizationRequestProcessingError.InvalidRequest);
            if (string.IsNullOrWhiteSpace(request.RedirectUri))
                throw new AuthorizationRequestProcessingException(_localizer["RequiredParameterNotSpecified", "redirect_uri"], AuthorizationRequestProcessingError.InvalidRequest);
            if (string.IsNullOrWhiteSpace(request.Scope))
                throw new AuthorizationRequestProcessingException(_localizer["RequiredParameterNotSpecified", "scope"], AuthorizationRequestProcessingError.InvalidRequest);
        }
    }
}
