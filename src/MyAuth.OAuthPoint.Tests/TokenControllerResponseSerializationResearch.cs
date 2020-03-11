using System;
using System.Collections.Generic;
using System.Text;
using MyAuth.OAuthPoint.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Xunit;
using Xunit.Abstractions;

namespace MyAuth.OAuthPoint.Tests
{
    public class TokenControllerResponseSerializationResearch
    {
        private readonly ITestOutputHelper _output;

        /// <summary>
        /// Initializes a new instance of <see cref="TokenControllerResponseSerializationResearch"/>
        /// </summary>
        public TokenControllerResponseSerializationResearch(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ShouldSerializeSuccessResponse()
        {
            //Arrange
            var request = new SuccessTokenResponse
            {
                RefreshToken = "YmE4ZGU2YzRjY2Y4NDU5NTlhMmQwMWY3NGVmZjIzMjY",
                AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1cm46dGVzdC1jbGFpbXM6Zm9vIjoiZm9vIiwidXJuOnRlc3QtY2xhaW1zOmJhciI6ImJhciIsIm5iZiI6MTU4MzMxNTcyNywiZXhwIjoxNTgzNDAyMTI3LCJpc3MiOiJpc3N1ZXIiLCJhdWQiOiJhdWRpZW5jZSJ9.MmiFj0dTd5e0JsKOd8KzIKuY_e4iop2wMjDoYyaiUaE",
                ExpiresIn = 60,
                TokenType = "bearer"
            };

            //Act
            var json = JsonConvert.SerializeObject(request, Formatting.Indented);
            _output.WriteLine(json);


            //Assert

        }

        [Fact]
        public void ShouldSerializeFailedResponse()
        {
            //Arrange
            var request = new ErrorTokenResponse
            {
                ErrorCode = TokenResponseErrorCode.InvalidClient,
                ErrorDescription = "Client not found"
            };

            //Act
            var json = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new StringEnumConverter { CamelCaseText = true } },
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });
            _output.WriteLine(json);


            //Assert

        }
    }
}
