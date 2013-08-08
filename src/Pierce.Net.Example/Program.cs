using System;
using System.Threading;
using Pierce.Net;

namespace Pierce.Net.Example
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var queue = new RequestQueue();

            var google = new Uri("http://www.google.com/");
            var songkick = new Uri("http://api.songkick.com/api/3.0/events.json?location=clientip&apikey=G2KCF6q91g23Q6Zh");

            for (var x = 1; x <= 16; x++)
            {
                Thread.Sleep(200);

                var request_number = x;
                var uri = x % 2 == 0 ?
                    google :
                    songkick;

                var request = new StringRequest
                {
                    Uri = uri,
                    OnResponse = response =>
                    {
                        Console.WriteLine("received response {0}, {1}", request_number, response.Substring(0, 48));
                    },
                    OnError = error =>
                    {
                        Console.WriteLine("[{0}] ERROR: {1}", request_number, error);
                    },
                };

                queue.Add(request);
            };

            Console.ReadLine();
        }
    }
}
