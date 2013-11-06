using Pierce.Json;
using Pierce.Json.SimpleJson;
using System.Text;

namespace Pierce.Net
{
    public class JsonRequest<T> : Request<T>
    {
        private readonly IJsonSerializer _serializer;
        private string _body;

        public JsonRequest(IJsonSerializer serializer = null)
        {
            this._serializer = serializer ?? new SimpleJsonSerializer();
        }

        public override string BodyContentType
        {
            get { return "text/json"; } // XXX?
        }

        public override string Body
        {
            get { return _body; }
        }

        public object ObjectBody
        {
            set { _body = _serializer.Serialize(value); }
        }

        public override Response Parse(NetworkResponse response)
        {
            var json_response = Encoding.UTF8.GetString(response.Data);

            // XXX: need to add and raise ParseException
            return new Response<T>
            {
                CacheEntry = CacheEntry.Create(response),
                Result = _serializer.Deserialize<T>(json_response),
            };
        }
    }
}
