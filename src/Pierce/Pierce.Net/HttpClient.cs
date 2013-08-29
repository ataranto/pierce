using System;
using System.Net;

namespace Pierce.Net
{
    public class HttpClient
    {
        public NetworkResponse Execute(Request request, WebHeaderCollection cache_headers)
        {
            var response = new NetworkResponse();

            try
            {
                var timeout_ms = request.RetryPolicy.CurrentTimeoutMs;
                var client = new TimeoutWebClient(timeout_ms);

                response.Data = client.DownloadData(request.Uri);
                response.StatusCode = HttpStatusCode.OK;
                response.Headers = client.ResponseHeaders;
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.Timeout)
                {
                    throw new TimeoutError();
                }
            }

            return response;
        }
    }
}

