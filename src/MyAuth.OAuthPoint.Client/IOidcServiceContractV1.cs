using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MyLab.ApiClient;

namespace MyAuth.OAuthPoint.Client
{
    /// <summary>
    /// OpenID Connect endpoints
    /// </summary>
    [Api("oidc/v1", Key = "oidc/v1")]
    public interface IOidcServiceContractV1
    {
        /// <summary>
        /// `authorization` endpoint
        /// </summary>
        /// <remarks>
        /// https://openid.net/specs/openid-connect-core-1_0.html#AuthRequest
        /// </remarks>
        [ExpectedCode(HttpStatusCode.Redirect)]
        [Get("authorization")]
        Task Authorization(
            [Query("response_type")]string responseType,
            [Query("client_id")] string clientId,
            [Query("redirect_uri")] string redirectUri,
            [Query("scope")] string scope,
            [Query("state")] string state = null,
            [Header("Cookie")] string cookie = null);
    }
}
