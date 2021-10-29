using System.Threading.Tasks;
using LinqToDB.Data;
using MyAuth.OAuthPoint.Db;
using MyAuth.OAuthPoint.Tools;
using MyLab.DbTest;

namespace FuncTests
{
    public class DataDbInitializer : ITestDbInitializer
    {
        public ClientDb[] Clients { get; set; }
        public ClientAvailableScopeDb[] ClientScopes { get; set; }
        public ClientAvailableUriDb[] ClientRedirectUris { get; set; }

        public LoginSessionDb[] LoginSessions { get; set; }
        public TokenSessionDb[] TokenSessions { get; set; }

        public async Task InitializeAsync(DataConnection dataConnection)
        {
            if (Clients != null)
                await dataConnection.BulkCopyAsync(Clients);

            if (ClientScopes != null)
                await dataConnection.BulkCopyAsync(ClientScopes);

            if (ClientRedirectUris != null)
                await dataConnection.BulkCopyAsync(ClientRedirectUris);

            if (LoginSessions != null)
                await dataConnection.BulkCopyAsync(LoginSessions);

            if (TokenSessions != null)
                await dataConnection.BulkCopyAsync(TokenSessions);

        }

        public static DataDbInitializer Create(string clientId, string redirectUri, string scope)
        {
            return new DataDbInitializer
            {
                Clients = new[] { new ClientDb { Id = clientId, Name = "foo", PasswordHash = TestTools.ClientPasswordHash} },
                ClientScopes = new[] { new ClientAvailableScopeDb{ ClientId = clientId, Name = scope } },
                ClientRedirectUris = new[] { new ClientAvailableUriDb { ClientId = clientId, Uri = redirectUri } }
            };
        }
    }
}