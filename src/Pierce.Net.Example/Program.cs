using System;
using Pierce.Net;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Pierce.Net.Example
{
    class JsonDateTime
    {
        public string Time { get; set; }
        public string Date { get; set; }
    }

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
            var jsontest = new Uri("http://date.jsontest.com");

            Parallel.For(0, 32, async x =>
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
                }
                catch (Exception ex)
                {
                    Console.WriteLine("handled exception: {0}", ex);
                }
            });
        }
    }
}
