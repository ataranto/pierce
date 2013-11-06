namespace Pierce.Json.SimpleJson
{
    public class SimpleJsonSerializer : IJsonSerializer
    {
        private static readonly IJsonSerializerStrategy _strategy = new SerializationStrategy();

        public string Serialize(object @object)
        {
            return Pierce.SimpleJson.SerializeObject(@object, _strategy);
        }

        public T Deserialize<T>(string json)
        {
            return Pierce.SimpleJson.DeserializeObject<T>(json, _strategy);
        }
    }
}
