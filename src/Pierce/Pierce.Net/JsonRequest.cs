using Pierce.Json;
using Pierce.Json.SimpleJson;
using System;
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
            get { return "application/json"; }
        }

        public override string Body
        {
            get { return _body; }
        }

        public object Data
        {
            set { _body = _serializer.Serialize(value); }
        }

        public override Response Parse(NetworkResponse response)
        {
            return new Response<T>
            {
                CacheEntry = CacheEntry.Create(response),
                Result = ParseJson(response),
            };
        }

        private T ParseJson(NetworkResponse response)
        {
            // XXX: can't assume utf8 encoding, need a parseCharset equiv
            var json_response = Encoding.UTF8.GetString(response.Data);

            try
            {
                return _serializer.Deserialize<T>(json_response);
            }
            catch (Exception ex)
            {
                throw new ParseException(ex, response);
            }
        }
    }
}
