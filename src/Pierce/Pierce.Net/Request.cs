using System;
using System.Globalization;
using System.Linq;
using Pierce.Logging;
using System.Threading.Tasks;

namespace Pierce.Net
{
    public abstract class Request
    {
        private readonly MarkerLog _marker_log = new MarkerLog();

        public Request()
        {
            Method = RequestMethod.Get;
            Priority = Priority.Normal;
            ShouldCache = true;
            RetryPolicy = new RetryPolicy();
        }

        public string Method { get; set; }
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

        public virtual object CacheKey
        {
            get { return Uri; }
        }

        public abstract Response Parse(NetworkResponse response);
        public abstract void SetResponse(Response response);
        public abstract void SetError(Error error);

        public virtual void Cancel()
        {
            IsCanceled = true;
        }

        public void AddMarker(string name)
        {
            _marker_log.Add(name);
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
        private static string date_format = "ddd, dd MMM yyyy hh:mm:ss GMT";
        private readonly TaskCompletionSource<T> _source = new TaskCompletionSource<T>();

        public async Task<T> GetResultAsync()
        {
            return await _source.Task;
        }

        public override sealed void SetResponse(Response response)
        {
            var typed_response = response as Response<T>;
            var result = typed_response.Result;

            _source.SetResult(result);
        }

        public override sealed void SetError(Error error)
        {
            _source.SetException(error);
        }

        public override sealed void Cancel()
        {
            base.Cancel();
            _source.SetCanceled();
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
}

