using Newtonsoft.Json;

namespace Pierce.Json
{
    public class JsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerSettings _settings =
            new DefaultSerializerSettings();

        string IJsonSerializer.Serialize(object @object)
        {
            return JsonConvert.SerializeObject(@object, _settings);
        }

        T IJsonSerializer.Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _settings);
        }
    }
}
