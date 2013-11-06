using Pierce.Json;
using Pierce.Json.SimpleJson;
using System.Text;

namespace Pierce.Net
{
    public class JsonRequest<T> : Request<T>
    {
        private readonly IJsonSerializer _serializer;

        public JsonRequest(IJsonSerializer serializer = null)
        {
            this._serializer = serializer ?? new SimpleJsonSerializer();
        }

        public override Response Parse(NetworkResponse response)
        {
            var json = Encoding.UTF8.GetString(response.Data);

            return new Response<T>
            {
                CacheEntry = CacheEntry.Create(response),
                Result = _serializer.Deserialize<T>(json),
            };
        }
    }
}
