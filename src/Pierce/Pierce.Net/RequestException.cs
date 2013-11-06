using System;

namespace Pierce.Net
{
    public class RequestException : Exception
    {
        public NetworkResponse Response { get; set; }

        public RequestException()
        {

        }

        public RequestException(Exception exception)
        {

        }
    }
}

