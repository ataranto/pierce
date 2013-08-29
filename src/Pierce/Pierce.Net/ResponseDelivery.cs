using System;

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
}

