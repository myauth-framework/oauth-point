using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace MyAuth.OAuthPoint.Tests
{
    public class JwtSecurityTokenResearch 
    {
        private readonly ITestOutputHelper _output;

        public JwtSecurityTokenResearch(ITestOutputHelper output)
        {
            _output = output;
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
        }

        [Fact]
        public void SerializationTest()
        {
            //Arrange
            var secret = Encoding.UTF8.GetBytes("very-secret-pass");
            var securityKey = new SymmetricSecurityKey(secret);
            var signCred = new SigningCredentials(securityKey, "HS256");
            var header = new JwtHeader(signCred);

            var claimes = new List<Claim>();
            claimes.Add(new Claim("urn:test-claimes:foo", "foo"));
            claimes.Add(new Claim("urn:test-claimes:bar", "bar"));

            var payload = new JwtPayload("issuer", "audience", claimes, DateTime.Now, DateTime.Now.AddDays(1));

            var token = new JwtSecurityToken(
                header, payload
                );

            //Act
            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            _output.WriteLine(tokenStr);
        }
    }
}
