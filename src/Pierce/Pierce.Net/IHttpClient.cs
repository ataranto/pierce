using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Pierce.Logging;

namespace Pierce.Net
{
    // XXX: ResponseScheduler? "Post" is a javaism
    public class ResponseDelivery
    {
        private readonly Action<Action> _invoke;

        public ResponseDelivery(Action<Action> invoke = null)
        {
            _invoke = invoke ?? (action => action());
        }

        public void PostResponse(Request request, Response response, Action action = null)
        {
            request.AddMarker("post-response");
            _invoke(() => Deliver(request, response, action));
        }

        public void PostError(Request request, Error error)
        {
            request.AddMarker("post-error");
            var response = new Response { Error = error };
            _invoke(() => Deliver(request, response));
        }

        private static void Deliver(Request request, Response response, Action action = null)
        {
            if (request.IsCanceled)
            {
                request.Finish("canceled-at-delivery");
                return;
            }

            if (response.IsSuccess)
            {
                request.SetResponse(response);
            }
            else
            {
                request.SetError(response.Error);
            }

            if (response.IsIntermediate)
            {
                request.AddMarker("intermediate-response");
            }
            else
            {
                request.Finish("done");
            }

            if (action != null)
            {
                action();
            }
        }
    }

    public enum Priority
    {
        Low,
        Normal,
        High,
    }

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

    public class TimeoutError : Error
    {
        
    }

    public class ConnectionError : Error
    {

    }

    public class RetryPolicy
    {
        public int CurrentRetryCount { get; private set; }
        public int CurrentTimeoutMs { get; private set; } // XXX TimeSpan?

        private readonly int _max_retries;
        private readonly double _backoff_multiplier;

        public RetryPolicy(int initial_timeout_ms = 2500, int max_retries = 1, double backoff_multiplier = 1)
        {
            CurrentTimeoutMs = initial_timeout_ms;
            _max_retries = max_retries;
            _backoff_multiplier = backoff_multiplier;
        }

        public void Retry(Error error)
        {
            CurrentRetryCount++;
            CurrentTimeoutMs += (int)Math.Round(CurrentTimeoutMs * _backoff_multiplier);

            if (CurrentRetryCount > _max_retries)
            {
                throw error;
            }
        }
    }

    public abstract class Request
    {
        private readonly MarkerLog _marker_log = new MarkerLog();

        public Request()
        {
            Priority = Priority.Normal;
            ShouldCache = true;
            RetryPolicy = new RetryPolicy();
        }

        public Uri Uri { get; set; }
        public Priority Priority { get; set; }
        public int Sequence { get; set; }
        public object Tag { get; set; }
        public CacheEntry CacheEntry { get; set; }
        public RequestQueue RequestQueue { get; set; }
        public bool ShouldCache { get; set; }
        public bool IsCanceled { get; private set; }
        public RetryPolicy RetryPolicy { get; set; }
        public bool ResponseDelievered { get; set; }
        public Action<Error> OnError { get; set; }

        public virtual object CacheKey
        {
            get { return Uri; }
        }

        public abstract Response Parse(NetworkResponse response);
        public abstract void SetResponse(Response response);

        public void AddMarker(string name)
        {
            _marker_log.Add(name);
        }

        public void Cancel()
        {
            IsCanceled = true;
        }

        public void SetError(Error error)
        {
            var action = OnError;

            if (action != null)
            {
                action(error);
            }
        }

        public void Finish(string marker_name)
        {
            if (RequestQueue == null)
            {
                return;
            }

            RequestQueue.Finish(this);

            AddMarker(marker_name);
            _marker_log.Finish(RequestQueue.Log, this.ToString());
        }

        public override string ToString()
        {
            return String.Format("[{0}] {1} {2} {3}",
                IsCanceled ? "X" : " ", Uri, Priority, Sequence);
        }
    }

    public abstract class Request<T> : Request
    {
        public Action<T> OnResponse { get; set; }

        private static string date_format = "ddd, dd MMM yyyy hh:mm:ss GMT";

        public override sealed void SetResponse(Response response)
        {
            var typed_response = response as Response<T>;
            var result = typed_response.Result;
            var action = OnResponse;

            if (action != null)
            {
                action(result);
            }

        }

        // XXX: should be in Response ctor or static Create() method? see Response.success()
        protected static CacheEntry GetCacheEntry(NetworkResponse response)
        {
            var headers = response.Headers;

            long server_date = 0;
            long server_expires = 0;
            long soft_expires = 0;
            long max_age = 0;
            var has_cache_control = false;

            var value = headers.Get("Date");
            if (value != null)
            {
                server_date = ParseDate(value);
            }

            value = headers.Get("Cache-Control");
            if (value != null)
            {
                has_cache_control = true;
                foreach (var token in value.Split(',').Select(x => x.Trim()))
                {
                    if (token == "no-cache" || token == "no-store")
                    {
                        return null;
                    }
                    else if (token.StartsWith("max-age="))
                    {
                        Int64.TryParse(token.Substring(8), out max_age);
                    }
                }
            }

            value = headers.Get("Expires");
            if (value != null)
            {
                server_expires = ParseDate(value);
            }

            if (has_cache_control)
            {
                var now = DateTime.Now.Ticks;
                soft_expires = now + max_age * 1000;
            }
            else if (server_date > 0 && server_expires > + server_date)
            {
                soft_expires = (server_expires - server_date);
            }

            return new CacheEntry
            {
                Data = response.Data,
                ETag = headers.Get("ETag"),
                Expires = soft_expires,
                SoftExpires = soft_expires,
                ServerDate = server_date,
                Headers = response.Headers,
            };
        }

        private static long ParseDate(string @string)
        {
            try
            {
                var provider = CultureInfo.InvariantCulture;
                var date = DateTime.ParseExact(@string, date_format, provider);

                return date.Ticks;
            }
            catch
            {
                return 0;
            }
        }
    }

    public class StringRequest : Request<string>
    {
        public override Response Parse(NetworkResponse response)
        {
            return new Response<string>
            {
                CacheEntry = GetCacheEntry(response),
                Result = Encoding.UTF8.GetString(response.Data),
            };
        }
    }

    public class Response
    {
        public CacheEntry CacheEntry { get; set; }
        public Error Error { get; set; }
        public bool IsIntermediate { get; set; }

        public bool IsSuccess
        {
            get { return Error == null; }
        }
    }

    public class Response<T> : Response
    {
        public T Result { get; set; }
    }

    public class NetworkResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public byte[] Data { get; set; }
        public WebHeaderCollection Headers { get; set; }
    }

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

    public class Network
    {
        private readonly ILog _log;
        private readonly HttpClient _client;

        public Network(ILog log, HttpClient client = null)
        {
            if (log == null)
            {
                new ArgumentNullException("log");
            }

            _log = log;
            _client = client ?? new HttpClient();
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

    public class CacheEntry
    {
        public byte[] Data { get; set; }
        public string ETag { get; set; }
        public long Expires { get; set; }
        public long SoftExpires { get; set; }
        public long ServerDate { get; set; }
        public WebHeaderCollection Headers { get; set; }

        public bool IsExpired
        {
            get { return Expires < DateTime.Now.Ticks; }
        }

        public bool ShouldRefresh
        {
            get { return SoftExpires < DateTime.Now.Ticks; }
        }
    }

    public class Cache
    {
        private readonly IDictionary<object, CacheEntry> _dictionary = new Dictionary<object, CacheEntry>();

        public CacheEntry this [object key]
        {
            get
            {
                lock (_dictionary)
                {
                    return _dictionary.ContainsKey(key) ? _dictionary[key] : null;
                }
            }

            set
            {
                lock (_dictionary)
                {
                    _dictionary[key] = value;
                }
            }
        }
    }

    public class RequestQueue
    {
        private readonly BlockingCollection<Request> _cache_queue = new BlockingCollection<Request>();
        private readonly BlockingCollection<Request> _network_queue = new BlockingCollection<Request>();

        private readonly ISet<Request> _requests = new HashSet<Request>();
        private readonly IDictionary<object, List<Request>> _blocked_requests = new Dictionary<object, List<Request>>();

        private readonly Cache _cache;
        private readonly Network _network;
        private readonly ResponseDelivery _delivery;

        private int _sequence;

        public RequestQueue(ILog log = null, Cache cache = null, Network network = null, ResponseDelivery delivery = null)
        {
            Log = log ?? new ConsoleLog { Tag = GetType().Namespace };
            _cache = cache ?? new Cache();
            _network = network ?? new Network(Log);
            _delivery = delivery ?? new ResponseDelivery();

            Task.Factory.StartNew(CacheConsumer);

            Task.Factory.StartNew(NetworkConsumer);
            Task.Factory.StartNew(NetworkConsumer);
        }

        public ILog Log { get; private set; }

        public Request Add(Request request)
        {
            lock (_requests)
            {
                _requests.Add(request);
            }

            request.RequestQueue = this;
            request.Sequence = Interlocked.Increment(ref _sequence);
            request.AddMarker("add-to-queue");

            if (!request.ShouldCache)
            {
                _network_queue.Add(request);
                return request;
            }

            lock (_blocked_requests)
            {
                List<Request> list;
                if (_blocked_requests.TryGetValue(request.CacheKey, out list))
                {
                    list = list ?? new List<Request>();
                    list.Add(request);

                    _blocked_requests[request.CacheKey] = list;
                }
                else
                {
                    _blocked_requests[request.CacheKey] = null;
                    _cache_queue.Add(request);
                }                
            }

            return request;
        }

        public void Cancel(Func<Request, bool> filter)
        {
            lock (_requests)
            {
                foreach (var request in _requests.Where(filter))
                {
                    request.Cancel();
                }
            }
        }

        public void Cancel(object tag)
        {
            if (tag == null)
            {
                throw new InvalidOperationException("tag");
            }

            Cancel(request => request.Tag == tag);
        }

        public void Finish(Request request)
        {
            lock (_requests)
            {
                _requests.Remove(request);
            }

            if (request.ShouldCache)
            {
                List<Request> list;

                lock (_blocked_requests)
                {
                    if (_blocked_requests.TryGetValue(request.CacheKey, out list) &&
                        _blocked_requests.Remove(request.CacheKey) &&
                        list != null)
                    {
                        list.ForEach(_cache_queue.Add);
                    }
                }
            }
        }

        private void CacheConsumer()
        {
            foreach (var request in _cache_queue.GetConsumingEnumerable())
            {
                request.AddMarker("cache-queue-dequeue");

                if (request.IsCanceled)
                {
                    request.Finish("cache-discard-canceled");
                    continue;
                }

                CacheEntry entry = _cache[request.CacheKey];
                if (entry == null)
                {
                    request.AddMarker("cache-miss");
                    _network_queue.Add(request);
                    continue;
                }

                if (entry.IsExpired)
                {
                    request.AddMarker("cache-hit-expired");
                    request.CacheEntry = entry;
                    _network_queue.Add(request);
                    continue;
                }

                request.AddMarker("cache-hit");
                var response = request.Parse(new NetworkResponse
                {
                    Data = entry.Data,
                    Headers = entry.Headers,
                });

                request.AddMarker("cache-hit-parsed");

                if (!entry.ShouldRefresh)
                {
                    _delivery.PostResponse(request, response);
                }
                else
                {
                    request.AddMarker("cache-hit-refresh-needed");
                    request.CacheEntry = entry;
                    response.IsIntermediate = true;

                    _delivery.PostResponse(request, response, () => _network_queue.Add(request));
                }
            }
        }

        private void NetworkConsumer()
        {
            foreach (var request in _network_queue.GetConsumingEnumerable())
            {
                request.AddMarker("network-dequeue");

                if (request.IsCanceled)
                {
                    request.Finish("network-discard-canceled");
                    continue;
                }

                try
                {
                    var network_response = _network.Execute(request);
                    request.AddMarker("network-http-complete");

                    if (network_response.StatusCode == HttpStatusCode.NotModified &&
                        request.ResponseDelievered)
                    {
                        request.Finish("not-modified");
                        continue;
                    }

                    var response = request.Parse(network_response);
                    request.AddMarker("network-parse-complete");

                    if (request.ShouldCache && response.CacheEntry != null)
                    {
                        _cache[request.CacheKey] = response.CacheEntry;
                        request.AddMarker("network-cache-written");
                    }

                    request.ResponseDelievered = true;
                    _delivery.PostResponse(request, response);
                }
                catch (Error ex)
                {
                    _delivery.PostError(request, ex);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Exception");

                    var error = new Error(ex);
                    _delivery.PostError(request, error);
                }
            }
        }
    }
}

