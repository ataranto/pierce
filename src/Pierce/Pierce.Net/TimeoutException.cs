using System;

namespace Pierce.Net
{
    public class TimeoutException : RequestException
    {
        public TimeoutException(Exception exception)
            : base("Request timed out", exception, null)
        {

        }
    }
}

