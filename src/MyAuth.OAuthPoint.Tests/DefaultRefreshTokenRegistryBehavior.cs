using MyAuth.OAuthPoint.Services;
using MyLab.RedisManager;
using Xunit;

namespace MyAuth.OAuthPoint.Tests
{
    public class DefaultRefreshTokenRegistryBehavior
    {
        [Fact]
        public void ShouldProvideRegisteredToken()
        {
            //Arrange
            var options = new RedisOptions
            {
                Host = "localhost"
            };
            IRedisManager redisManager = new DefaultRedisManager(options);
            IRefreshTokenRegistry refreshTokenRegistry = new DefaultRefreshTokenRegistry(redisManager);
            
            //Act
            

            //Assert

        }
    }
}