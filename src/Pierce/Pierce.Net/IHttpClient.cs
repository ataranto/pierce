using System.Net;

namespace Pierce.Net
{
    public interface IHttpClient
    {
        NetworkResponse Execute(Request request, WebHeaderCollection cache_headers);
    }
}

