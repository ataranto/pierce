using System;

namespace Pierce.Net
{
    public class RequestException : Exception
    {
        public NetworkResponse Response { get; set; }

        public RequestException(string message, Exception exception, NetworkResponse response)
            : base(message, exception)
        {
            Response = response;
        }
    }
}

