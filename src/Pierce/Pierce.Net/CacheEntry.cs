using System;
using System.Globalization;
using System.Linq;
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

        public static CacheEntry Create(NetworkResponse response)
        {
            long server_date = 0;
            long server_expires = 0;
            long soft_expires = 0;
            long max_age = 0;
            var has_cache_control = false;

            var value = response.Headers.Get("Date");
            if (value != null)
            {
                server_date = ParseDate(value);
            }

            value = response.Headers.Get("Cache-Control");
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

            value = response.Headers.Get("Expires");
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
                ETag = response.Headers.Get("ETag"),
                Expires = soft_expires,
                SoftExpires = soft_expires,
                ServerDate = server_date,
                Headers = response.Headers,
            };
        }

        private static string date_format = "ddd, dd MMM yyyy hh:mm:ss GMT";
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

