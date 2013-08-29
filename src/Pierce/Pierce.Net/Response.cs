namespace Pierce.Net
{
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
}

