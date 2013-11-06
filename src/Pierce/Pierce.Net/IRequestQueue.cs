using System;

namespace Pierce.Net
{
    public interface IRequestQueue
    {
        Request Add(Request request);
        void Cancel(Func<Request, bool> filter);
        void Cancel(object tag);
    }
}
