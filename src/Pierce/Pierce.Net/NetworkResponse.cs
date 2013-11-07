using System.Net;

namespace Pierce.Net
{
    public class NetworkResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public WebHeaderCollection Headers { get; set; }
        public byte[] Data { get; set; }
    }
}

