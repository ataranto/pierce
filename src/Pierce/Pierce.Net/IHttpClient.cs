using System.Net;

namespace Pierce.Net
{
    public interface IHttpClient
    {
        CookieContainer CookieContainer { get; set; }
        NetworkResponse Execute(Request request, WebHeaderCollection headers);
    }
}

