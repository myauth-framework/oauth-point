using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Tools.TokenIssuing;
using Newtonsoft.Json;
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

        [Fact]
        public void ShouldCreateTokenWithDigitalExpirationClaims()
        {
            //Arrange
            DateTime expiry = DateTime.Parse("2021-11-02 08:03:08");
            DateTime issuedAt = DateTime.Parse("2021-11-02 00:00:00");
            
            var basicClaims = new BaseClaimSet
            {
                Subject = "foo",
                Expiry = expiry,
                IssuedAt = issuedAt
            };

            var f = new AccessTokenFactory(basicClaims, null);

            //Act
            var token = f.Create("1234567890123456");

            _output.WriteLine(token);

            var tokenHandler = new JwtSecurityTokenHandler();
            var actualToken = tokenHandler.ReadJwtToken(token);

            var expClaim = actualToken.Claims.FirstOrDefault(c => c.Type == "exp");
            var iatClaim = actualToken.Claims.FirstOrDefault(c => c.Type == "iat");

            //Assert
            Assert.NotNull(expClaim);
            Assert.Equal("1635829388", expClaim.Value);
            Assert.NotEqual(ClaimValueTypes.String, expClaim.ValueType);
            Assert.Equal(ClaimValueTypes.Integer, expClaim.ValueType);

            Assert.NotNull(iatClaim);
            Assert.Equal("1635800400", iatClaim.Value);
            Assert.NotEqual(ClaimValueTypes.String, iatClaim.ValueType);
            Assert.Equal(ClaimValueTypes.Integer, iatClaim.ValueType);
        }

        [Fact]
        public void ShouldSerializeClaimValueObjectAsJsonArray()
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

            var arrClaim = ClaimValue.Parse("[1, 2, 3]");

            var addClaims = new ClaimsCollection(new Dictionary<string, ClaimValue>
            {
                {"foo-claim", arrClaim}
            });

            var f = new AccessTokenFactory(basicClaims, addClaims);

            //Act
            var token = f.Create("1234567890123456");

            _output.WriteLine(token);

            var tokenHandler = new JwtSecurityTokenHandler();
            var actualToken = tokenHandler.ReadJwtToken(token);

            var payloadJson = actualToken.Payload.SerializeToJson();
            _output.WriteLine("");
            _output.WriteLine(payloadJson);

            var fooClaimValue = JsonConvert.DeserializeObject<ClaimValue>(actualToken.Payload["foo-claim"].ToString());

            int[] restoredArray = fooClaimValue.Array?.Values<int>().ToArray();

            //Assert
            Assert.DoesNotContain("\"[1, 2, 3]\"", payloadJson);
            Assert.NotNull(restoredArray);
            Assert.Equal(1,restoredArray[0]);
            Assert.Equal(2,restoredArray[1]);
            Assert.Equal(3,restoredArray[2]);

        }
    }
}
