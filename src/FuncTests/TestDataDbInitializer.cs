using System.Threading.Tasks;
using LinqToDB.Data;
using MyAuth.OAuthPoint.Db;
using MyLab.DbTest;

namespace FuncTests
{
    public class DataDbInitializer : ITestDbInitializer
    {
        public ClientDb[] Clients { get; set; }
        public ClientScopeDb[] ClientScopes { get; set; }
        public ClientRedirectUriDb[] ClientRedirectUris { get; set; }

        public LoginSessionDb[] LoginSessions { get; set; }
        public SessionInitiationDb[] SessionInitiations { get; set; }

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

            if (SessionInitiations != null)
                await dataConnection.BulkCopyAsync(SessionInitiations);
        }

        public static DataDbInitializer Create(string clientId, string redirectUri, string scope)
        {
            return new DataDbInitializer
            {
                Clients = new[] { new ClientDb { Id = clientId, Name = "foo" } },
                ClientScopes = new[] { new ClientScopeDb { ClientId = clientId, ScopeName = scope } },
                ClientRedirectUris = new[] { new ClientRedirectUriDb { ClientId = clientId, Uri = redirectUri } }
            };
        }
    }
}