using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using MyAuth.OAuthPoint.Db;
using MyAuth.OAuthPoint.Models.DataContract;
using MyLab.Db;
using Newtonsoft.Json.Linq;

namespace MyAuth.OAuthPoint.Tools
{
    class SuccessfulLoginApplyLogic
    {
        private readonly string _sessionId;
        private readonly DataConnection _dc;
        private readonly LoginSuccessRequest _succReq;

        public SuccessfulLoginApplyLogic(
            string sessionId,
            DataConnection dc,
            LoginSuccessRequest succReq)
        {
            _sessionId = sessionId;
            _dc = dc;
            _succReq = succReq;
        }

        public Task CreateSubjectIfNotExistsAsync()
        {
            return _dc.Tab<SubjectDb>().InsertOrUpdateAsync(
                () => new SubjectDb
                {
                    Id = _succReq.Subject,
                    Enabled = MySqlBool.True
                },
                s => s);
        }

        public Task RemoveSubjectIdentityClaimsAsync()
        {
            return _dc.Tab<SubjectIdentityClaimDb>()
                .Where(c => c.SubjectId == _succReq.Subject)
                .DeleteAsync();
        }

        public async Task SaveIdentityClaimsAsync()
        {
            var identityClaims = _succReq.IdentityScopes?.SelectMany(sc =>
                    sc.Claims.Select(c =>
                        new SubjectIdentityClaimDb
                        {
                            Name = c.Key,
                            Value = c.Value.ToString(),
                            ScopeId = sc.Id,
                            SubjectId = _succReq.Subject
                        }))
                .ToArray();

            if (identityClaims != null && identityClaims.Length != 0)
                await _dc.BulkCopyAsync(identityClaims);
        }

        public Task RemoveSubjectAccessClaimsAsync()
        {
            return _dc.Tab<SubjectAccessClaimDb>()
                .Where(c => c.SubjectId == _succReq.Subject)
                .DeleteAsync();
        }

        public async Task SaveAccessClaimsAsync()
        {
            var accessClaims = _succReq.AccessClaims
                ?.Select(cl =>
                    new SubjectAccessClaimDb
                    {
                        Name = cl.Key,
                        Value = cl.Value.ToString(),
                        SubjectId = _succReq.Subject
                    })
                .ToArray();

            if (accessClaims != null && accessClaims.Length != 0)
                await _dc.BulkCopyAsync(accessClaims);
        }

        public Task UpdateSessionStateAsync(string authCode, DateTime authCodeExpiry)
        {
            return _dc.PerformAutoTransactionAsync(async d =>
            {
                await _dc.Tab<LoginSessionDb>()
                    .Where(s => s.Id == _sessionId && s.Status == LoginSessionDbStatus.Pending)
                    .Set(s => s.SubjectId, () => _succReq.Subject)
                    .Set(s => s.Status, () => LoginSessionDbStatus.Started)
                    .UpdateAsync();

                await _dc.Tab<TokenSessionDb>()
                    .Where(s => s.LoginId == _sessionId && s.Status == TokenSessionDbStatus.Pending)
                    .Set(s => s.Status, () => TokenSessionDbStatus.Started)
                    .Set(s => s.AuthCode, () => authCode)
                    .Set(s => s.AuthCodeExpiry, () => authCodeExpiry)
                    .UpdateAsync();
            });
        }
    }
}