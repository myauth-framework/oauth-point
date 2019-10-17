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
        public void ShouldVerifySelfSign()
        {
            //Arrange
            var secret = Encoding.UTF8.GetBytes("very-secret-pass");
            var securityKey = new SymmetricSecurityKey(secret);
            var signCred = new SigningCredentials(securityKey, "HS256");
            var header = new JwtHeader(signCred);

            var claims = new List<Claim>();
            claims.Add(new Claim("urn:test-claims:foo", "foo"));
            claims.Add(new Claim("urn:test-claims:bar", "bar"));

            var payload = new JwtPayload("issuer", "audience", claims, DateTime.Now, DateTime.Now.AddDays(1));

            var token = new JwtSecurityToken(
                header, payload
                );

            var wth = new JwtSecurityTokenHandler();

            var tokenStr = wth.WriteToken(token);
            _output.WriteLine(tokenStr);

            //Act
            wth.ValidateToken(tokenStr, new TokenValidationParameters
            {
                IssuerSigningKey = securityKey,
                ValidateIssuerSigningKey = true,
                ValidateAudience = false,
                ValidateIssuer = false
            }, out _);
        }
    }
}
