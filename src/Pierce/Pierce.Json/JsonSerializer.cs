using Newtonsoft.Json;

namespace Pierce.Json
{
    public class JsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerSettings _settings;

        public JsonSerializer()
        {
            _settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new UnderscoreContractResolver(),
            };
        }

        string IJsonSerializer.Serialize(object @object)
        {
            var json = JsonConvert.SerializeObject(@object, _settings);
            System.Console.WriteLine("JSON: {0}", json);

            return json;
        }

        T IJsonSerializer.Deserialize<T>(string json)
        {
            var @object = JsonConvert.DeserializeObject<T>(json, _settings);
            System.Console.WriteLine("OBJ : {0}", @object);

            return @object;
        }
    }
}
