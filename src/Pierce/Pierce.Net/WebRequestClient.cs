using System;
using System.Net;
using System.IO;
using System.Reflection;

namespace Pierce.Net
{
    public class WebRequestClient : IHttpClient
    {
        private static readonly MethodInfo AddWithoutValidate = typeof(WebHeaderCollection).
            GetMethod("AddWithoutValidate", BindingFlags.Instance | BindingFlags.NonPublic);

        public NetworkResponse Execute(Request request, WebHeaderCollection cache_headers)
        {
            var web_request = WebRequest.Create(request.Uri) as HttpWebRequest;
            web_request.KeepAlive = true;
            web_request.Method = request.Method;
            web_request.Timeout = request.RetryPolicy.CurrentTimeoutMs;

            foreach (var key in cache_headers.AllKeys)
            {
                var parameters = new object[] { key, cache_headers[key] };
                AddWithoutValidate.Invoke(web_request.Headers, parameters);
            }

            if (request.Method == "POST" || request.Method == "PUT")
            {
                web_request.ContentType = request.BodyContentType;

                using (var request_stream = web_request.GetRequestStream())
                using (var stream_writer = new StreamWriter(request_stream))
                {
                    stream_writer.Write(request.Body);
                }
            }
                  
            try
            {
                using (var response = web_request.GetResponse() as HttpWebResponse)
                using (var response_stream = response.GetResponseStream())
                using (var memory_stream = new MemoryStream())
                {
                    response_stream.CopyTo(memory_stream);

                    return new NetworkResponse
                    {
                        StatusCode = response.StatusCode,
                        Headers = response.Headers,
                        Data = memory_stream.ToArray(),
                    };
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.Timeout)
                {
                    throw new TimeoutException(ex);
                }

                throw;
            }
        }
    }
}

