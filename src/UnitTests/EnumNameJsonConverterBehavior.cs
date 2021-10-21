using MyAuth.OAuthPoint.Tools;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class EnumNameJsonConverterBehavior
    {
        private readonly ITestOutputHelper _output;

        public EnumNameJsonConverterBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ShouldConvertEnumWithName()
        {
            //Arrange
            var model = new TestModel
            {
                Enum = TestEnum.Item2
            };

            //Act
            var strModel = JsonConvert.SerializeObject(model);

            _output.WriteLine(strModel);

            var restoredModel = JsonConvert.DeserializeObject<TestModel>(strModel);

            //Assert
            Assert.Equal(TestEnum.Item2, restoredModel.Enum);
        }

        class TestModel
        {
            [JsonConverter(typeof(EnumNameJsonConverter))]
            public TestEnum Enum { get; set; }
        }

        enum TestEnum
        {
            Item1,
            [EnumName("foo_bar")]
            Item2
        }
    }
}
