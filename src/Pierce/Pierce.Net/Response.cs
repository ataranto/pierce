namespace Pierce.Net
{
    public class Response
    {
        public CacheEntry CacheEntry { get; set; }
        public RequestException Exception { get; set; }
        public bool IsIntermediate { get; set; }

        public bool IsSuccess
        {
            get { return Exception == null; }
        }
    }

    public class Response<T> : Response
    {
        public T Result { get; set; }
    }
}

