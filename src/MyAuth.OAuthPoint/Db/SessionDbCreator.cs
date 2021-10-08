using System;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using MyAuth.OAuthPoint.Models;
using MyLab.Db;

namespace MyAuth.OAuthPoint.Db
{
    class SessionDbCreator
    {
        private readonly DataConnection _dataConnection;
        private readonly string _loginSessionId;
        private readonly DateTime _expiry;
        private readonly AuthorizationRequest _authorizationRequest;

        public SessionDbCreator(DataConnection dataConnection, 
            string loginSessionId, 
            DateTime expiry,
            AuthorizationRequest authorizationRequest)
        {
            _dataConnection = dataConnection;
            _loginSessionId = loginSessionId;
            _expiry = expiry;
            _authorizationRequest = authorizationRequest;
        }

        public  Task Create()
        {
            return _dataConnection.PerformAutoTransactionAsync(async c =>
            {
                await c.Tab<LoginSessionDb>()
                    .InsertAsync(() => new LoginSessionDb
                    {
                        Id = _loginSessionId,
                        Expiry = _expiry,
                        ClientId = _authorizationRequest.ClientId,
                    });

                await c.Tab<SessionInitiationDb>()
                    .InsertAsync(() =>  new SessionInitiationDb
                        {
                            SessionId = _loginSessionId,
                            RedirectUri = _authorizationRequest.RedirectUri,
                            State = _authorizationRequest.State,
                            Scope = _authorizationRequest.Scope
                        }
                    );
            });
        }
    }
}
