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
    }
}

