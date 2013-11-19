using System;

namespace Pierce.Net
{
    // XXX: ResponseScheduler? "Post" is a javaism
    public class ResponseDelivery : IResponseDelivery
    {
        public void PostResponse(Request request, Response response, Action action = null)
        {
            request.AddMarker("post-response");
            Deliver(request, response, action);
        }

        public void PostException(Request request, RequestException exception)
        {
            request.AddMarker("post-exception");
            var response = new Response { Exception = exception };
            Deliver(request, response);
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
                request.SetException(response.Exception);
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
}

