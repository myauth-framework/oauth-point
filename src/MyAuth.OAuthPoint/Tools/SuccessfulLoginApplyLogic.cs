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
                    Id = _succReq.Subject
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
            var identityClaims = _succReq.IdentityScopes.SelectMany(sc =>
                    sc.Claims.Select(c =>
                        new SubjectIdentityClaimDb
                        {
                            Name = c.Key,
                            Value = c.Value.ToString(),
                            ScopeId = sc.Id,
                            SubjectId = _succReq.Subject
                        }))
                .ToArray();

            if (identityClaims.Length != 0)
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
                .Select(cl =>
                    new SubjectAccessClaimDb
                    {
                        Name = cl.Key,
                        Value = cl.Value.ToString(),
                        SubjectId = _succReq.Subject
                    })
                .ToArray();

            if (accessClaims.Length != 0)
                await _dc.BulkCopyAsync(accessClaims);
        }

        public Task UpdateSessionStateAsync()
        {
            return _dc.Tab<LoginSessionDb>()
                .Where(s => s.Id == _sessionId)
                .Set(s => s.SubjectId, s => _succReq.Subject)
                .Set(s => s.Completed, s => MySqlBool.True)
                .Set(s => s.CompletedDt, s => DateTime.Now)
                .UpdateAsync();
        }


        //string ClaimValueToString(JObject claimValue) => claimValue.ToString().Trim('\"');
    }
}