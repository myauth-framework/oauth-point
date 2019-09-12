using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DotRedis;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace MyAuth.OAuthPoint.Tests
{
    public class LoginControllerBehavior : IClassFixture<TestWebApplicationFactory>
    {
        private readonly TestWebApplicationFactory _factory;
        private readonly ITestOutputHelper _output;

        public LoginControllerBehavior(
            TestWebApplicationFactory factory,
            ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
        }

        [Theory]
        [MemberData(nameof(GetLoginFiles), "Invalid")]
        public async Task ShouldFailInvalidRequests(string requestFile) 
        {
            //Arrange
            var client = _factory.CreateClient();
            var filename = Path.Combine(GetLoginFilesDir("Invalid"), requestFile);
            var request = new StringContent(File.ReadAllText(filename));
            request.Headers.Remove("Content-Type");
            request.Headers.Add("Content-Type", "application/json");
            
            //Act
            var resp = await client.PostAsync("/login", request);
            var msg = await resp.Content.ReadAsStringAsync();
            _output.WriteLine(msg);
            
            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }
        
        [Theory]
        [MemberData(nameof(GetLoginFiles), "Valid")]
        public async Task ShouldPassValidRequests(string requestFile) 
        {
            //Arrange
            var client = _factory.CreateClient();
            var filename = Path.Combine(GetLoginFilesDir("Valid"), requestFile);
            var requestContentStr = File.ReadAllText(filename);
            var request = new StringContent(requestContentStr);
            request.Headers.Remove("Content-Type");
            request.Headers.Add("Content-Type", "application/json");
            
            //Act
            var resp = await client.PostAsync("/login", request);
            var msg = await resp.Content.ReadAsStringAsync();
            _output.WriteLine(msg);
            
            //Assert
            Assert.True(resp.IsSuccessStatusCode);
        }

        public static object[][] GetLoginFiles(string relPath)
        {
            var dirPath = GetLoginFilesDir(relPath);
            return Directory.GetFiles(dirPath).Select(f => new object[]
            {
                Path.GetFileName(f)
            }).ToArray();
        }

        static string GetLoginFilesDir(string relPath) => 
            Path.Combine(Directory.GetCurrentDirectory(), "Requests", "Login", relPath);
    }
}