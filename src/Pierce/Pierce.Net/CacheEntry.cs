using System;
using System.Net;

namespace Pierce.Net
{
    public class CacheEntry
    {
        public byte[] Data { get; set; }
        public string ETag { get; set; }
        public long Expires { get; set; }
        public long SoftExpires { get; set; }
        public long ServerDate { get; set; }
        public WebHeaderCollection Headers { get; set; }

        public bool IsExpired
        {
            get { return Expires < DateTime.Now.Ticks; }
        }

        public bool ShouldRefresh
        {
            get { return SoftExpires < DateTime.Now.Ticks; }
        }
    }
}

