using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MyAuth.OAuthPoint.Models;

namespace MyAuth.OAuthPoint.Tools
{
    class AccessTokenBuilder
    {
        public JwtPayload Payload { get; private set; }

        public JwtHeader Header { get; private set; }

        public void CreateHeader(string secret)
        {
            var binSecret = Encoding.UTF8.GetBytes(secret);

            Header = new JwtHeader(new SigningCredentials(new SymmetricSecurityKey(binSecret), "HS256"));
        }

        public void CreatePayload(LoginRequest loginRequest, string issuer, string[] audience, int accessTokenLifeTimeMin)
        {
            var claims = new List<Claim>();
            claims.AddRange(loginRequest.Claims.Select(c => new Claim(c.Name, c.Value)));
            claims.AddRange(loginRequest.Roles.Select(c => new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", c)));
            claims.Add(new Claim("sub", loginRequest.Subject));

            var a = audience == null || audience.Length == 0 
            ? null
            : string.Join(' ', audience);

            Payload = new JwtPayload(issuer, a, claims, DateTime.Now, DateTime.Now.AddMinutes(accessTokenLifeTimeMin));
        }

        public JwtSecurityToken Build()
        {
            return new JwtSecurityToken(Header, Payload);
        }

        public string BuildString()
        {
            return new JwtSecurityTokenHandler().WriteToken(Build());
        }
    }
}