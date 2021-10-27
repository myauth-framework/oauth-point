using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using MyAuth.OAuthPoint.Db;
using MyLab.Db;

namespace MyAuth.OAuthPoint.Services
{
    public interface ILogoutService
    {
        Task LogoutAsync(string sessionId);
    }

    class LogoutService : ILogoutService
    {
        private readonly IDbManager _dbManager;

        public LogoutService(IDbManager dbManager)
        {
            _dbManager = dbManager;
        }
        public async Task LogoutAsync(string sessionId)
        { 
            await _dbManager.DoOnce().Tab<LoginSessionDb>()
                .Where(s => s.Id == sessionId && s.Expiry < DateTime.Now && s.Completed != MySqlBool.True)
                .Set(s => s.Revoked, MySqlBool.True)
                .UpdateAsync();
        }
    }
}
