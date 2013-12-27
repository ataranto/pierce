using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.IO;

namespace Pierce.Json.Bson
{
    public class BsonSerializer : IBsonSerializer
    {
        private readonly JsonSerializerSettings _settings;

        public BsonSerializer()
        {
            _settings = new DefaultSerializerSettings();
        }

        public BsonSerializer(JsonSerializerSettings settings)
        {
            _settings = settings;
        }

        byte[] IBsonSerializer.Serialize(object @object)
        {
            var stream = new MemoryStream();
            var writer = new BsonWriter(stream);
            var serializer = Newtonsoft.Json.JsonSerializer.Create(_settings);

            serializer.Serialize(writer, @object);
            return stream.ToArray();
        }

        T IBsonSerializer.Deserialize<T>(byte[] data)
        {
            var stream = new MemoryStream(data);
            var reader = new BsonReader(stream);
            var serializer = Newtonsoft.Json.JsonSerializer.Create(_settings);

            return serializer.Deserialize<T>(reader);
        }
    }
}
