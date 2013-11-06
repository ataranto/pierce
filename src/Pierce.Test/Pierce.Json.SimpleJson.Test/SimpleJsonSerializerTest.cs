using System.Runtime.Serialization;
using Xunit;

namespace Pierce.Json.SimpleJson.Test
{
    public class SimpleJsonSerializerTest
    {
        [Fact]
        public void Deserialize_InvalidJson_RaisesException()
        {
            var serializer = new SimpleJsonSerializer();
            Assert.Throws<SerializationException>(() =>
                serializer.Deserialize<object>("invalid json"));
        }
    }
}
