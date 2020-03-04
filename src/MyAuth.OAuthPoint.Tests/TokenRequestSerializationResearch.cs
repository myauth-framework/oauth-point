using System;
using MyAuth.OAuthPoint.Models;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace MyAuth.OAuthPoint.Tests
{
    public class TokenRequestSerializationResearch
    {
        private readonly ITestOutputHelper _output;

        /// <summary>
        /// Initializes a new instance of <see cref="TokenRequestSerializationResearch"/>
        /// </summary>
        public TokenRequestSerializationResearch(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ShouldSerializeIssueRequest()
        {
            //Arrange
            var request = new TokenRequest
            {
                AuthCode = Guid.NewGuid().ToString("N"),
                ClientId = "android-default",
                CodeVerifier = "qwerty",
                GrantType = "authorization_code"
            };

            //Act
            var json = JsonConvert.SerializeObject(request, Formatting.Indented);
            _output.WriteLine(json);

            //Assert
        }

        [Fact]
        public void ShouldSerializeReissueRequest()
        {
            //Arrange
            var request = new TokenRequest
            {
                RefreshToken = "YmE4ZGU2YzRjY2Y4NDU5NTlhMmQwMWY3NGVmZjIzMjY",
                ClientId = "android-default",
                CodeVerifier = "qwerty",
                GrantType = "refresh_token"
            };

            //Act
            var json = JsonConvert.SerializeObject(request, Formatting.Indented);
            _output.WriteLine(json);

            //Assert
        }
    }
}