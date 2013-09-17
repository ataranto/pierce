using System.Text;

namespace Pierce.Net
{
    public class StringRequest : Request<string>
    {
        public override Response Parse(NetworkResponse response)
        {
            return new Response<string>
            {
                CacheEntry = CacheEntry.Create(response),
                Result = Encoding.UTF8.GetString(response.Data),
            };
        }
    }
}

