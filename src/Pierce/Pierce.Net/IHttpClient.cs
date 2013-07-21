using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pierce.Net
{
    public enum Priority
    {
        Low,
        Normal,
        High,
    }

    public abstract class Request
    {
        public Request()
        {
            Priority = Priority.Normal;
            ShouldCache = true;
        }

        public Uri Uri { get; set; }
        public Priority Priority { get; set; }
        public int Sequence { get; set; }
        public object Tag { get; set; }
        public RequestQueue RequestQueue { get; set; }
        public bool ShouldCache { get; set; }
        public bool IsCanceled { get; private set; }

        public virtual object CacheKey
        {
            get { return Uri; }
        }

        public abstract Response Parse(NetworkResponse response);
        public abstract void SetResponse(Response response);

        public void Cancel()
        {
            IsCanceled = true;
        }

        public void Finish()
        {
            if (RequestQueue != null)
            {
                RequestQueue.Finish(this);
            }
        }

        public override string ToString()
        {
            return String.Format("{0}:{1}", Sequence, Uri);
        }
    }

    public abstract class Request<T> : Request
    {
        public Action<T> OnResponse { get; set; }

        public override void SetResponse(Response response)
        {
            if (IsCanceled)
            {
                Finish();
                return;
            }

            var typed_response = response as Response<T>;
            var result = typed_response.Result;

            OnResponse(result);
            Finish();
        }

        // XXX: should be in Response ctor or static Create() method? see Response.success()
        protected static CacheEntry GetCacheEntry(NetworkResponse response)
        {
            return new CacheEntry
            {
                Data = response.Data,
                Headers = response.Headers,
            };
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
        //public bool NotModified { get; set; }
    }

    public class HttpClient
    {
        public NetworkResponse Execute(Request request, WebHeaderCollection cache_headers)
        {
            var response = new NetworkResponse();

            try
            {
                var client = new WebClient();
                response.Data = client.DownloadData(request.Uri);
                response.StatusCode = HttpStatusCode.OK;
            }
            catch
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
            }

            return response;
        }
    }

    public class Network
    {
        private readonly HttpClient _client;

        public Network(HttpClient client = null)
        {
            _client = client ?? new HttpClient();
        }

        public NetworkResponse Execute(Request request)
        {
            try
            {
                // XXX: need request.CacheEntry, add cache headers
                var cache_headers = new WebHeaderCollection();
                var response = _client.Execute(request, cache_headers);

                return response;
            }
            catch
            {
                throw;
            }
        }
    }

    public class CacheEntry
    {
        public byte[] Data { get; set; }
        public string ETag { get; set; }
        public long ServerDate { get; set; }
        public WebHeaderCollection Headers { get; set; }
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

        private int sequence;

        public RequestQueue(Cache cache = null, Network network = null)
        {
            _cache = cache ?? new Cache();
            _network = network ?? new Network();

            Task.Factory.StartNew(CacheConsumer);

            Task.Factory.StartNew(NetworkConsumer);
            Task.Factory.StartNew(NetworkConsumer);
        }

        public Request Add(Request request)
        {
            request.RequestQueue = this;
            request.Sequence = Interlocked.Increment(ref sequence);
            Console.WriteLine("Add(): {0}", request);

            lock (_requests)
            {
                _requests.Add(request);
            }

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
                if (request.IsCanceled)
                {
                    request.Finish();
                    continue;
                }

                CacheEntry entry = _cache[request.CacheKey];
                if (entry == null)
                {
                    _network_queue.Add(request);
                    continue;
                }

                var response = request.Parse(new NetworkResponse
                {
                    Data = entry.Data,
                    Headers = entry.Headers,
                });

                request.SetResponse(response);
            }
        }

        private void NetworkConsumer()
        {
            foreach (var request in _network_queue.GetConsumingEnumerable())
            {
                Console.WriteLine("NetworkConsumer: {0}", request);

                if (request.IsCanceled)
                {
                    request.Finish();
                    continue;
                }

                try
                {
                    var network_response = _network.Execute(request);
                    var response = request.Parse(network_response);

                    if (request.ShouldCache && response.CacheEntry != null)
                    {
                        _cache[request.Uri] = response.CacheEntry;
                    }

                    request.SetResponse(response);
                }
                catch (Exception ex)
                {
                    // XXX
                    Console.WriteLine(ex);
                }
            }
        }
    }
}

