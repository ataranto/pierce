using System;
using System.Net;

namespace Pierce.Net
{
    public class TimeoutWebClient : WebClient
    {
        private readonly int _timeout_ms;

        public TimeoutWebClient(int timeout_ms)
        {
            _timeout_ms = timeout_ms;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            request.Timeout = _timeout_ms;

            return request;
        }
    }
}

