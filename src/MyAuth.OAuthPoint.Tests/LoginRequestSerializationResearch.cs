using MyAuth.OAuthPoint.Models;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace MyAuth.OAuthPoint.Tests
{
    public class LoginRequestSerializationResearch
    {
        private readonly ITestOutputHelper _output;

        /// <summary>
        /// Initializes a new instance of <see cref="LoginRequestSerializationResearch"/>
        /// </summary>
        public LoginRequestSerializationResearch(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ShouldSerialize()
        {
            //Arrange
            var request = new LoginRequest
            {
                Subject = "user-id",
                Audience = new []{ "host1.com", "host2.com" },
                ClientId = "android-default",
                RedirectUri = "http://host1.com/auth-code",
                Roles = new [] {"admin"},
                Claims = new []
                {
                    new LoginRequest.Claim
                    {
                        Name = "name",
                        Value = "Jhon"
                    },
                    new LoginRequest.Claim
                    {
                        Name = "surname",
                        Value = "Ololoevich"
                    },
                },
                CodeChallengeMethod = "md5",
                CodeChallenge = "2FeO34RYzgb7xbt2pYxcpA=="
            };

            //Act
            var json = JsonConvert.SerializeObject(request, Formatting.Indented);
            _output.WriteLine(json);

            //Assert
        }
    }
}
