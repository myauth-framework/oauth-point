using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using MyAuth.OAuthPoint.Db;
using MyLab.DbTest;

namespace FuncTests
{
    public class MyAuthOAuthPointDbInitializer : ITestDbInitializer
    {
        public async Task InitializeAsync(DataConnection dataConnection)
        {
            await dataConnection.CreateTableAsync<LoginSessionDb>();
            await dataConnection.CreateTableAsync<SessionScopeDb>();
            await dataConnection.CreateTableAsync<SessionInitiationDb>();
            await dataConnection.CreateTableAsync<ClaimDb>();
            await dataConnection.CreateTableAsync<ClientDb>();
            await dataConnection.CreateTableAsync<ClientRedirectUriDb>();
            await dataConnection.CreateTableAsync<ClientScopeDb>();
        }
    }
}