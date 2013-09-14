using System;
using System.IO;
using System.Net;
using Pierce.Logging;

namespace Pierce.Net
{
    public class Network
    {
        private readonly ILog _log;
        private readonly IHttpClient _client;

        public Network(ILog log, IHttpClient client = null)
        {
            if (log == null)
            {
                throw new ArgumentNullException("log");
            }

            _log = log;
            _client = client ?? new WebRequestClient();
        }

        public NetworkResponse Execute(Request request)
        {
            while (true)
            {
                NetworkResponse response = null;

                try
                {
                    var cache_headers = GetCacheHeaders(request.CacheEntry);
                    response = _client.Execute(request, cache_headers);

                    if (response.StatusCode == HttpStatusCode.NotModified)
                    {
                        return new NetworkResponse
                        {
                            StatusCode = response.StatusCode,
                            Data = request.CacheEntry.Data,
                            Headers = response.Headers,
                        };
                    }

                    if (response.StatusCode != HttpStatusCode.OK &&
                        response.StatusCode != HttpStatusCode.NoContent)
                    {
                        throw new IOException();
                    }

                    return response;
                }
                catch (TimeoutError error)
                {
                    AttemptRetry(request, error);
                }
                catch (IOException ex)
                {
                    if (response == null)
                    {
                        throw new ConnectionError();
                    }

                    _log.Error("Unexpected response code {0} for {1}", response.StatusCode, request.Uri);
                    throw; // XXX logic
                }
            }
        }

        private static void AttemptRetry(Request request, Error error)
        {
            var retry_policy = request.RetryPolicy;
            var timeout_ms = retry_policy.CurrentTimeoutMs;

            try
            {
                retry_policy.Retry(error);
                request.AddMarker(String.Format("retry [timeout={0}]", timeout_ms));
            }
            catch (Error)
            {
                request.AddMarker(String.Format("timeout-giveup [timeout={0}]", timeout_ms));
                throw;
            }
        }


        private static WebHeaderCollection GetCacheHeaders(CacheEntry entry)
        {
            var headers = new WebHeaderCollection();

            if (entry == null)
            {
                return headers;
            }

            if (entry.ETag != null)
            {
                headers.Add(HttpRequestHeader.IfNoneMatch, entry.ETag);
            }

            if (entry.ServerDate > 0)
            {
                // XXX: no clue if this is the correct date format
                var date = new DateTime(entry.ServerDate).ToShortDateString();
                headers.Add(HttpRequestHeader.IfModifiedSince, date);
            }

            return headers;
        }
    }
}

