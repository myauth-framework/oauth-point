using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using DotRedis.Connection;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Services;
using MyAuth.OAuthPoint.Tools;
using MyLab.RedisManager;
using Xunit;
using Xunit.Sdk;

namespace MyAuth.OAuthPoint.Tests
{
    public class DefaultRefreshTokenRegistryBehavior
    {
        private const string Subject = "Subject";
        private static readonly string Token1 = Guid.NewGuid().ToString();
        private static readonly string Token2 = Guid.NewGuid().ToString();
        private static readonly DefaultRefreshTokenRegistry Registry;
        private static readonly DefaultRedisManager RedisManager;

        static DefaultRefreshTokenRegistryBehavior()
        {
            var options = new RedisOptions
            {
                Host = "localhost"
            };
            RedisManager = new DefaultRedisManager(options);
            Registry = new DefaultRefreshTokenRegistry(RedisManager);
        }
        
        [Fact]
        [CleanupKeysAttribute]
        public async Task ShouldProvideRegisteredToken()
        {
            //Arrange
            var tokenDesc = new RefreshTokenDescription
            {
                LoginRequest = new LoginRequest
                {
                    Subject = "uid"
                },
                NotAfter = DateTime.Now.AddMinutes(1)
            };

            //Act
            
            await Registry.Register(Token1, tokenDesc);
            var storedTokenDesc = await Registry.Get(Token1);

            //Assert
            Assert.NotNull(storedTokenDesc);
            Assert.Equal("uid", storedTokenDesc.Subject);
        }
        
        [Fact]
        [CleanupKeysAttribute]
        public async Task ShouldProvideNullIfTokenNotRegistered()
        {
            //Arrange
            
            //Act
            var storedTokenDesc = await Registry.Get("foo");

            //Assert
            Assert.Null(storedTokenDesc);
        }
        
        [Fact]
        [CleanupKeysAttribute]
        public async Task ShouldRemoveToken()
        {
            //Arrange
            var tokenDesc = new RefreshTokenDescription
            {
                LoginRequest = new LoginRequest
                {
                    Subject = "uid"
                },
                NotAfter = DateTime.Now.AddMinutes(1)
            };

            //Act
            
            await Registry.Register(Token1, tokenDesc);
            await Registry.Remove(Token1);
            var storedTokenDesc = await Registry.Get(Token1);

            //Assert
            Assert.Null(storedTokenDesc);
        }
        
        [Fact]
        [CleanupKeysAttribute]
        public async Task ShouldRemoveBySubject()
        {
            //Arrange
            var tokenDesc = new RefreshTokenDescription
            {
                LoginRequest = new LoginRequest
                {
                    Subject = Subject
                },
                NotAfter = DateTime.Now.AddMinutes(1)
            };

            //Act
            
            await Registry.Register(Token1, tokenDesc);
            await Registry.Register(Token2, tokenDesc);
            await Registry.RemoveBySubject(Subject);
            var storedToken1Desc = await Registry.Get(Token1);
            var storedToken2Desc = await Registry.Get(Token2);

            //Assert
            Assert.Null(storedToken1Desc);
            Assert.Null(storedToken2Desc);
        }

        class CleanupKeysAttribute : BeforeAfterTestAttribute
        {   
            public override void After(MethodInfo methodUnderTest)
            {
                using (var c = RedisManager.GetConnection().Result)
                {
                     c.DeleteKeysAsync(
                        RefreshTokenRedisKey.Create(Token1),
                        RefreshTokenRedisKey.Create(Token2)).RunSynchronously();
                }
            }   
        }
    }
}