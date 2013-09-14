using System;
using System.Globalization;
using System.Linq;
using Pierce.Logging;

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
}

