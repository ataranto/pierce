using System;
using Pierce.Net;
using System.Threading.Tasks;

namespace Pierce.Net.Example
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Init();
            Console.ReadLine();
        }

        public static async void Init()
        {
            var queue = new RequestQueue();

            var google = new Uri("http://www.google.com/");
            var songkick = new Uri("http://api.songkick.com/api/3.0/events.json?location=clientip&apikey=G2KCF6q91g23Q6Zh");

            Parallel.For(1, 16, async x =>
            {
                var uri = x % 2 == 0 ?
                    google :
                    songkick;

                var request = new StringRequest
                {
                    Uri = uri,
                };
                queue.Add(request);

                try
                {
                    var result = await request.GetResultAsync();
                    Console.WriteLine("received response: {0}", result.Substring(0, 40));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("handled exception: {0}", ex);
                }
            });
        }
    }
}
