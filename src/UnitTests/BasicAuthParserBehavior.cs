using MyAuth.OAuthPoint.Tools;
using Xunit;

namespace UnitTests
{
    public class BasicAuthParserBehavior
    {
        [Fact]
        public void ShouldParse()
        {
            //Arrange
            var headerVal = "QWxhZGRpbjpvcGVuIHNlc2FtZQ==";
            
            //Act
            var parsed = BasicAuthParser.TryParse(headerVal, out var user, out var password);

            //Assert
            Assert.True(parsed);
            Assert.Equal("Aladdin", user);
            Assert.Equal("open sesame", password);
        }
    }
}
