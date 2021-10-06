using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyAuth.OAuthPoint.Tools;
using Xunit;

namespace UnitTests
{
    public class EnumNameAttributeBehavior
    {
        [Fact]
        public void ShouldProvideOverridenName()
        {
            //Arrange
            

            //Act
            var overridenName = EnumNameAttribute.GetName(TestEnum.Val1);
            
            //Assert
            Assert.Equal("foo", overridenName);
        }

        [Fact]
        public void ShouldProvideOriginalName()
        {
            //Arrange


            //Act
            var overridenName = EnumNameAttribute.GetName(TestEnum.Val2);

            //Assert
            Assert.Equal("Val2", overridenName);
        }

        enum TestEnum
        {
            [EnumName("foo")]
            Val1,
            Val2
        }
    }
}
