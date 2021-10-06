using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class JsoNSerializationBehavior
    {
        private readonly ITestOutputHelper _output;

        public JsoNSerializationBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ShouldNotUseBaseDictionaryForAdditionalProperties()
        {
            //Arrange
            var model = new TestModel
            {
                TestProperty = "foo"
            };

            model.Add("TestProperty2", "bar");

            //Act
            var str = JsonConvert.SerializeObject(model, Formatting.Indented);
            _output.WriteLine(str);
            var restoredModel = JsonConvert.DeserializeObject<TestModel>(str);

            //Assert
            Assert.Null(restoredModel.TestProperty);
            Assert.Single(restoredModel);
            Assert.Equal("bar", restoredModel["TestProperty2"]);
        }

        [Fact]
        public void ShouldDeserializeObjectValueAsStringPropertyValue()
        {
            //Arrange
            var strProp = new ModelWithProperty<TestEntity>
            {
                Prop = new TestEntity
                {
                    Id = "foo",
                    Value = "bar"
                }
            };

            var strValue = JsonConvert.SerializeObject(strProp);

            //Act
            var restored = JsonConvert.DeserializeObject<ModelWithProperty<object>>(strValue);

            var restoredValueType = restored.Prop.GetType();
            _output.WriteLine(restoredValueType.ToString());

            //Assert
            Assert.Equal(typeof(JObject), restoredValueType);
        }

        class TestModel : Dictionary<string, string>
        {
            public string TestProperty { get; set; }
        }

        class ModelWithProperty<T>
        {
            public T Prop { get; set; }
        }

        class TestEntity
        {
            public string Id { get; set; }
            public string Value { get; set; }
        }
    }
}
