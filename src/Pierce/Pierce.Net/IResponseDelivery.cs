using System;

namespace Pierce.Net
{
    public interface IResponseDelivery
    {
        void PostException(Request request, RequestException exception);
        void PostResponse(Request request, Response response, Action action = null);
    }
}
