using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DotRedis.Connection;
using DotRedis.ObjectModel;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Tools;
using MyLab.RedisManager;

namespace MyAuth.OAuthPoint.Services
{
    public interface IRefreshTokenRegistry
    {
        Task Register(string token, RefreshTokenDescription desc);

        Task<LoginRequest> Get(string token);

        Task Remove(string token);
        
        Task RemoveBySubject(string subject);
    }
    
    public class RefreshTokenDescription
    {
        public LoginRequest LoginRequest { get; set; }
        public DateTime NotAfter { get; set; }
    }
    
    class DefaultRefreshTokenRegistry : IRefreshTokenRegistry
    {
        public IRedisManager RedisManager { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="RefreshTokenRegistry"/>
        /// </summary>
        public DefaultRefreshTokenRegistry(IRedisManager redisManager)
        {
            RedisManager = redisManager;
        }
        
        public async Task Register(string token, RefreshTokenDescription desc)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            if (desc == null) throw new ArgumentNullException(nameof(desc));

            if(desc.NotAfter <= DateTime.Now)
                throw new InvalidOperationException("Token is expired");
            
            using (var c = await RedisManager.GetConnection())
            {
                await SaveToken(token, desc, c);

                await UpdateSubjectRefreshTokensList(token, desc, c);
            }
        }

        public async Task<LoginRequest> Get(string token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            
            using (var c = await RedisManager.GetConnection())
            {
                var tkKey = RefreshTokenRedisKey.Create(token);
                var tk = new RedisKey<RefreshTokenDescription>(tkKey, c);
                var refrTokenDesc = await tk.GetAsync();

                return refrTokenDesc?.LoginRequest;
            }
        }

        public async Task Remove(string token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            
            using (var c = await RedisManager.GetConnection())
            {
                var tkKey = RefreshTokenRedisKey.Create(token);
                await c.DeleteKeysAsync(tkKey);
            }
        }

        public async Task RemoveBySubject(string subject)
        {
            if (subject == null) throw new ArgumentNullException(nameof(subject));

            using (var c = await RedisManager.GetConnection())
            {
                var skKey = SubjectRefreshTokensRedisKey.Create(subject);
                var sk = new RedisKey<SubjectRefreshTokenList>(skKey, c);
                var subjectRefreshTokenList = await sk.GetAsync();

                if (subjectRefreshTokenList != null)
                {
                    var keysToRemove = subjectRefreshTokenList
                        .Select(itm => RefreshTokenRedisKey.Create(itm.RefreshToken));
                    await c.DeleteKeysAsync(keysToRemove);
                }

                await sk.DeleteAsync();
            }
        }
        
        private static async Task UpdateSubjectRefreshTokensList(string token, RefreshTokenDescription desc, IRedisConnection c)
        {
            var skKey = SubjectRefreshTokensRedisKey.Create(desc.LoginRequest.Subject);
            var sk = new RedisKey<SubjectRefreshTokenList>(skKey, c);
            var subjectRefreshTokenList = await sk.GetAsync();

            if (subjectRefreshTokenList != null)
            {
                var expiredRt = subjectRefreshTokenList
                    .Where(t => t.NotAfter < DateTime.Now)
                    .ToArray();
                foreach (var srt in expiredRt)
                    subjectRefreshTokenList.Remove(srt);
            }
            else
            {
                subjectRefreshTokenList = new SubjectRefreshTokenList();
            }

            subjectRefreshTokenList.Add(new SubjectRefreshToken
            {
                RefreshToken = token,
                NotAfter = desc.NotAfter
            });

            await sk.SetAsync(subjectRefreshTokenList);
        }

        private static async Task SaveToken(string token, RefreshTokenDescription desc, IRedisConnection c)
        {
            var tkKey = RefreshTokenRedisKey.Create(token);
            var tk = new RedisKey<RefreshTokenDescription>(tkKey, c);
            await tk.SetAsync(desc);
            await tk.SetExpirationAsync(desc.NotAfter - DateTime.Now);
        }

        class SubjectRefreshTokenList : Collection<SubjectRefreshToken>
        {
        }

        class SubjectRefreshToken
        {
            public string RefreshToken { get; set; }
            public DateTime NotAfter { get; set; }
        }
    }
}