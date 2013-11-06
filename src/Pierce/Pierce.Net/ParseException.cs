using System;

namespace Pierce.Net
{
    public class ParseException : RequestException
    {
        public ParseException(Exception exception, NetworkResponse response)
            : base("Failed to parse response", exception, response)
        {

        }
    }
}
