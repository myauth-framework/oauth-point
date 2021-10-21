using MyAuth.OAuthPoint.Tools;
using Xunit;

namespace UnitTests
{
    public class PasswordHashCalculatorBehavior
    {
        [Fact]
        public void ShouldCalcMd5WithSalt()
        {
            //Arrange
            var salt = "some-salt";
            var password = "custom-password";
            var expectedHash = "a6579e9b18558f14aeb0aa66f24c005d";

            var calculator = new PasswordHashCalculator(salt);

            //Act
            var actualHash = calculator.CalcHexPasswordMd5(password);

            //Assert
            Assert.Equal(expectedHash, actualHash);
        }
    }
}
