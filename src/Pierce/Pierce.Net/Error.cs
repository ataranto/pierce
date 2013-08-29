using System;

namespace Pierce.Net
{
    public class Error : Exception
    {
        public NetworkResponse Response { get; set; }

        public Error()
        {

        }

        public Error(Exception exception)
        {

        }
    }
}

