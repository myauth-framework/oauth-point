using System;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace MyAuth.OAuthPoint.Tests
{
    public class IncomingControllerBehavior : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public IncomingControllerBehavior(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public void ShouldCheckRequest()
        {
            //Arrange
            var client = _factory.CreateClient();

            //Act
            

            //Assert

        }
    }
}