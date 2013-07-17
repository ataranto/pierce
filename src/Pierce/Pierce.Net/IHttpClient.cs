using System;
using System.Net;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Reactive;
using System.Collections.Generic;

namespace Pierce.Net
{
    public class Request : IObservable<Response>
    {
        public Request()
        {
            ShouldCache = true;
            Subject = new Subject<Response>();
        }

        public Uri Uri { get; set; }
        public uint Sequence { get; set; }
        public bool ShouldCache { get; set; }
        public ISubject<Response> Subject { get; private set; }

        public IDisposable Subscribe(IObserver<Response> observer)
        {
            return Subject.Subscribe(observer);
        }
    }

    public class Response
    {
        public byte[] Data { get; set; }
        public Exception Exception { get; set; }
    }

    public class Network
    {
        public Response Execute(Request request)
        {
            var response = new Response();

            try
            {
                var client = new WebClient();
                response.Data = client.DownloadData(request.Uri);
            }
            catch (Exception ex)
            {
                response.Exception = ex;
            }

            return response;
        }
    }

    public class Cache
    {
        private readonly IDictionary<object, byte[]> _dictionary = new Dictionary<object, byte[]>();

        public bool TryGet(object key, out byte[] entry)
        {
            lock (_dictionary)
            {
                return _dictionary.TryGetValue(key, out entry);
            }
        }

        public void Put(object key, byte[] entry)
        {
            lock (_dictionary)
            {
                _dictionary[key] = entry;
            }
        }
    }

    public class RequestQueue
    {
        private readonly BlockingCollection<Request> _cache_queue = new BlockingCollection<Request>();
        private readonly BlockingCollection<Request> _network_queue = new BlockingCollection<Request>();

        private readonly Cache _cache;
        private readonly Network _network;

        public RequestQueue(Cache cache = null, Network network = null)
        {
            _cache = cache ?? new Cache();
            _network = network ?? new Network();

            Task.Factory.StartNew(CacheConsumer);
            Task.Factory.StartNew(NetworkConsumer);
        }

        public Request Add(Request request)
        {
            if (request.ShouldCache)
            {
                _cache_queue.Add(request);
            }
            else
            {
                _network_queue.Add(request);
            }

            return request;

        }

        private void CacheConsumer()
        {
            foreach (var request in _cache_queue.GetConsumingEnumerable())
            {
                byte[] data;
                if (_cache.TryGet(request.Uri, out data))
                {
                    var response = new Response { Data = data };
                    request.Subject.OnNext(response);
                }
                else
                {
                    _network_queue.Add(request);
                }
            }
        }

        private void NetworkConsumer()
        {
            foreach (var request in _network_queue.GetConsumingEnumerable())
            {
                Console.WriteLine("_network.Execute: {0}", request.Uri);
                var response = _network.Execute(request);

                if (request.ShouldCache)
                {
                    _cache.Put(request.Uri, response.Data);
                }

                request.Subject.OnNext(response);
            }
        }
    }
}

