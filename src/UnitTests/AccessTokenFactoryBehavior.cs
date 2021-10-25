using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Tools;
using MyAuth.OAuthPoint.Tools.TokenIssuing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class AccessTokenFactoryBehavior
    {
        private readonly ITestOutputHelper _output;

        public AccessTokenFactoryBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ShouldCreateToken()
        {
            //Arrange
            var basicClaims = new BaseClaimSet
            {
                Subject = "foo",
                Audiences = new[] { "host1", "host2" },
                Expiry = DateTime.Now.AddDays(1),
                Issuer = "bar",
                IssuedAt = DateTime.Now
            };
            var addClaims = new ClaimsCollection(new Dictionary<string, ClaimValue>
            {
                {"foo-claim", new ClaimValue("bar-claim")}
            });

            var f = new AccessTokenFactory(basicClaims, addClaims);

            //Act
            var token = f.Create("1234567890123456");

            _output.WriteLine(token);

            var tokenHandler = new JwtSecurityTokenHandler();
            var actualToken = tokenHandler.ReadJwtToken(token);

            //Assert
            Assert.Equal("foo", actualToken.Subject);
            Assert.Equal("bar", actualToken.Issuer);
            Assert.Contains("host1", actualToken.Audiences);
            Assert.Contains("host2", actualToken.Audiences);
            Assert.Contains(actualToken.Claims, c => c.Type == "foo-claim" && c.Value.ToString().Trim('\"') == "bar-claim");
        }
    }
}
