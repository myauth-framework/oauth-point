using System;
using System.Collections.Generic;
using MyAuth.OAuthPoint.Models;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class ClaimValueBehavior
    {
        private readonly ITestOutputHelper _output;

        public ClaimValueBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ShouldSerializeAnArray()
        {
            //Arrange
            var arrayValue = new []
            {
                new TestObject{Value = "foo"},
                new TestObject{Value = "bar"},
            };
            var arrayClaimValue = new ClaimValue(arrayValue);

            //Act
            var claimString = arrayClaimValue.ToString();

            _output.WriteLine(claimString);

            var restored = ClaimValue.Parse(claimString);

            var restoredArray = restored.Array.ToObject<TestObject[]>();

            //Assert
            Assert.NotNull(restoredArray);
            Assert.Equal("foo", restoredArray[0].Value);
            Assert.Equal("bar", restoredArray[1].Value);
        }

        [Theory]
        [MemberData(nameof(GetSerializationCases))]
        public void ShouldConvertValues(string f, ClaimValue claim)
        {
            //Arrange
            var scope = new TestScope{ Claim = claim };
            var scopeJson = JsonConvert.SerializeObject(scope, Formatting.Indented);

            _output.WriteLine(scopeJson);

            //Act
            var restored = JsonConvert.DeserializeObject<TestScope>(scopeJson);

            //Assert
            Assert.Equal(scope.Claim, restored.Claim);
        }

        [Theory]
        [MemberData(nameof(GetParseCases))]
        public void ShouldParseValues(string f, ClaimValue claim)
        {
            //Arrange

            //Act
            var claimString = claim.ToString();

            _output.WriteLine(claimString);

            var restored = ClaimValue.Parse(claimString);

            //Assert
            Assert.Equal(claim, restored);
        }

        public static IEnumerable<object[]> GetSerializationCases()
        {
            return new[]
            {
                new object[] {"string", new ClaimValue("foo")},
                new object[] {"int", new ClaimValue(123)},
                new object[] {"double", new ClaimValue(123d)},
                new object[] {"bool", new ClaimValue(true)},
                new object[] {"datetime", new ClaimValue(DateTime.Now)},
                new object[] {"object", new ClaimValue(new TestObject {Value = "bar"})},
            };
        }

        public static IEnumerable<object[]> GetParseCases()
        {
            return new[]
            {
                new object[] {"string", new ClaimValue("foo")},
                new object[] {"object", new ClaimValue(new TestObject {Value = "bar"})},
            };
        }

        class TestScope
        {
            public ClaimValue Claim { get; set; }
        }

        class TestObject
        {
            public string Value { get; set; }
        }
    }
}
