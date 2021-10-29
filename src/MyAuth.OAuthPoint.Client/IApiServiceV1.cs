using System.Threading.Tasks;
using MyAuth.OAuthPoint.Client.Models;
using MyLab.ApiClient;

namespace MyAuth.OAuthPoint.Client
{
    /// <summary>
    /// MyAuth API endpoints
    /// </summary>
    [Api("api/v1", Key = "api/v1")]
    public interface IApiServiceV1
    {
        /// <summary>
        /// Complete login process successfully
        /// </summary>
        [Post("login/{loginSessionId}/success")]
        Task SuccessLogin([Path] string loginSessionId, [JsonContent] LoginSuccessRequest authorizedSubjectInfo);

        /// <summary>
        /// Complete login process failed
        /// </summary>
        [Post("login/{loginSessionId}/error")]
        Task FailLogin([Path] string loginSessionId, [JsonContent] LoginErrorRequest loginErrorRequest);

        /// <summary>
        /// Return back from login process
        /// </summary>
        [Get("authorization-callback")]
        Task CallbackLogin([Query("login_id")]string loginId, [Query("client_id")] string clientId);
    }

}