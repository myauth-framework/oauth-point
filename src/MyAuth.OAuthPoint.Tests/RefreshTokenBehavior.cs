using MyAuth.OAuthPoint.Tools;
using Xunit;
using Xunit.Abstractions;

namespace MyAuth.OAuthPoint.Tests
{
    public class RefreshTokenBehavior
    {
        private readonly ITestOutputHelper _output;

        /// <summary>
        /// Initializes a new instance of <see cref="RefreshTokenBehavior"/>
        /// </summary>
        public RefreshTokenBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ShouldGenerateRefreshToken()
        {
            //Arrange

            //Act
            var oneDayRefreshToken = RefreshToken.Generate(1);
            _output.WriteLine(oneDayRefreshToken.Body);

            //Assert

        }
    }
}