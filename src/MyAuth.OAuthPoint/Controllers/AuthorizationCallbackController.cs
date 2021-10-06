﻿using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyAuth.OAuthPoint.Services;
using MyAuth.OAuthPoint.Tools;
using MyLab.Log.Dsl;
using MyLab.WebErrors;

namespace MyAuth.OAuthPoint.Controllers
{
    [Route("authorization-callback")]
    [ApiController]
    public class AuthorizationCallbackController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly IStringLocalizer<AuthorizationCallbackController> _localizer;
        private readonly AuthOptions _options;
        private readonly IDslLogger _log;

        public AuthorizationCallbackController(
            ILoginService loginService, 
            IOptions<AuthOptions> options, 
            ILogger<AuthorizationController> logger,
            IStringLocalizer<AuthorizationCallbackController> localizer)
        {
            _loginService = loginService;
            _localizer = localizer;
            _options = options.Value;
            _log = logger.Dsl();
        }

        [HttpGet]
        [ErrorToResponse(typeof(LoginSessionNotFoundException), HttpStatusCode.NotFound)]
        public IActionResult Get([FromQuery(Name = "login_session_id")] string loginId)
        {
            var lSession = _loginService.GetLoginSession(loginId);

            var sessState = lSession.InitDetails;

            if (sessState.Error != null)
            {
                return UrlRedirector.RedirectCallbackError(sessState.RedirectUri, sessState.Error.Error, sessState.Error.Description, sessState.State);
            }

            var authCookie = new LoginSessionCookie(loginId)
            {
                Expiry = TimeSpan.FromDays(_options.LoginExpiryDays)
            };
            authCookie.Save(Response);

            return UrlRedirector.RedirectSuccessCallback(sessState.RedirectUri, sessState.AuthorizationCode, sessState.State);
        }
    }
}
