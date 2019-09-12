using Xunit;

namespace MyAuth.OAuthPoint.Tests
{
    public class StringResearch
    {
        [Fact]
        public void ShouldNAMETrimMultipleChars()
        {
            //Arrange
            var testStr = "123===";

            //Act
            var trimmed = testStr.TrimEnd('=');

            //Assert
            Assert.Equal("123", trimmed);
        }
    }
}