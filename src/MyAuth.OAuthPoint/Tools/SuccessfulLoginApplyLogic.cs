using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using MyAuth.OAuthPoint.Db;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Models.DataContract;
using MyLab.Db;
using Newtonsoft.Json.Linq;

namespace MyAuth.OAuthPoint.Tools
{
    class SuccessfulLoginApplyLogic
    {
        private readonly DataConnection _dc;

        public SuccessfulLoginApplyLogic(
            DataConnection dc)
        {
            _dc = dc;
        }

        public Task CreateSubjectIfNotExistsAsync(string subject)
        {
            return _dc.Tab<SubjectDb>().InsertOrUpdateAsync(
                () => new SubjectDb
                {
                    Id = subject,
                    Enabled = MySqlBool.True
                },
                s => s);
        }

        public Task RemoveSubjectIdentityClaimsAsync(string subject)
        {
            return _dc.Tab<SubjectIdentityClaimDb>()
                .Where(c => c.SubjectId == subject)
                .DeleteAsync();
        }

        public async Task SaveIdentityClaimsAsync(string subject, ScopeClaims[] idScopes)
        {
            var identityClaims = idScopes?.SelectMany(sc =>
                    sc.Claims
                        .Where(c => c.Value != null && !c.Value.IsNull)
                        .Select(c =>
                        new SubjectIdentityClaimDb
                        {
                            Name = c.Key,
                            Value = c.Value.ToString(),
                            ScopeId = sc.Id,
                            SubjectId = subject
                        }))
                .ToArray();

            if (identityClaims != null && identityClaims.Length != 0)
                await _dc.BulkCopyAsync(identityClaims);
        }

        public Task RemoveSubjectAccessClaimsAsync(string subject)
        {
            return _dc.Tab<SubjectAccessClaimDb>()
                .Where(c => c.SubjectId == subject)
                .DeleteAsync();
        }

        public async Task SaveAccessClaimsAsync(string subject, ClaimsCollection accessClaims)
        {
            var accCl = accessClaims
                    ?.Where(cl => cl.Value != null && !cl.Value.IsNull)
                    .Select(cl =>
                    new SubjectAccessClaimDb
                    {
                        Name = cl.Key,
                        Value = cl.Value.ToString(),
                        SubjectId = subject
                    })
                .ToArray();

            if (accCl != null && accCl.Length != 0)
                await _dc.BulkCopyAsync(accCl);
        }

        public async Task UpdateLoginSessionStateAsync(string loginSessionId, string subject)
        {
            await _dc.Tab<LoginSessionDb>()
                .Where(s => s.Id == loginSessionId && s.Status == LoginSessionDbStatus.Pending)
                .Set(s => s.SubjectId, () => subject)
                .Set(s => s.Status, () => LoginSessionDbStatus.Started)
                .UpdateAsync();
        }

        public async Task UpdateTokenSessionStateAsync(string loginSessionId, string authCode, DateTime authCodeExpiry)
        {
            await _dc.Tab<TokenSessionDb>()
                .Where(s => s.LoginId == loginSessionId && s.Status == TokenSessionDbStatus.Pending)
                .Set(s => s.Status, () => TokenSessionDbStatus.Ready)
                .Set(s => s.AuthCode, () => authCode)
                .Set(s => s.AuthCodeExpiry, () => authCodeExpiry)
                .UpdateAsync();
        }
    }
}