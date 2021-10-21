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
            await dataConnection.CreateTableAsync<ClientDb>();
            await dataConnection.CreateTableAsync<ClientAvailableUriDb>();
            await dataConnection.CreateTableAsync<ClientAvailableScopeDb>();

            await dataConnection.CreateTableAsync<SubjectDb>();
            await dataConnection.CreateTableAsync<SubjectAccessClaimDb>();
            await dataConnection.CreateTableAsync<SubjectAvailableScopeDb>();
            await dataConnection.CreateTableAsync<SubjectIdentityClaimDb>();

            await dataConnection.CreateTableAsync<LoginSessionDb>();
        }
    }
}