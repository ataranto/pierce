using System;

namespace Pierce.Net
{
    public class ConnectionException : RequestException
    {
        public ConnectionException(Exception exception, NetworkResponse response)
            : base("Failed to connect to server", exception, response)
        {

        }
    }
}

