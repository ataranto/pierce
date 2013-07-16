using System;
using System.Net;
using System.Reactive.Subjects;
using System.Reactive.Linq;

namespace Pierce.Net
{
    public class Request
    {
        public Uri Uri { get; set; }
    }

    public class Response
    {
        public byte[] Data { get; set; }
        public Exception Exception { get; set; }
    }

    public class RequestExecutor
    {
        public Request Execute(Request request, ISubject<Response> subject)
        {
            var client = new WebClient();

            client.DownloadDataCompleted += client_DownloadDataCompleted;
            client.DownloadDataAsync(request.Uri, subject);

            return request;
        }

        private void client_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            var observer = e.UserState as IObserver<Response>;
            var response = new Response
            {
                Data = e.Result,
                Exception = e.Error,
            };

            observer.OnNext(response);
            observer.OnCompleted();
        }
    }

    public class RequestQueue
    {
        private readonly RequestExecutor _executor;

        public RequestQueue(RequestExecutor executor = null)
        {
            _executor = executor ?? new RequestExecutor();
        }

        public IObservable<Response> Add(Request request)
        {
            var subject = new Subject<Response>();
            _executor.Execute(request, subject);

            return subject;
        }
    }
}

