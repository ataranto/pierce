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
                NetworkResponse network_response = null;

                try
                {
                    var cache_headers = GetCacheHeaders(request.CacheEntry);
                    network_response = _client.Execute(request, cache_headers);

                    if (network_response.StatusCode == HttpStatusCode.NotModified)
                    {
                        return new NetworkResponse
                        {
                            StatusCode = network_response.StatusCode,
                            Data = request.CacheEntry.Data,
                            Headers = network_response.Headers,
                        };
                    }

                    if (network_response.StatusCode != HttpStatusCode.OK &&
                        network_response.StatusCode != HttpStatusCode.NoContent)
                    {
                        throw new IOException();
                    }

                    return network_response;
                }
                catch (TimeoutException ex)
                {
                    AttemptRetry(request, ex);
                }
                catch (IOException ex)
                {
                    if (network_response == null)
                    {
                        throw 
                            new ConnectionException(ex, network_response);
                    }

                    _log.Error("Unexpected response code {0} for {1}", network_response.StatusCode, request.Uri);
                    throw; // XXX logic
                }
            }
        }

        private static void AttemptRetry(Request request, RequestException exception)
        {
            var retry_policy = request.RetryPolicy;
            var timeout_ms = retry_policy.CurrentTimeoutMs;

            try
            {
                retry_policy.Retry(exception);
                request.AddMarker(String.Format("retry [timeout={0}]", timeout_ms));
            }
            catch (RequestException)
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

