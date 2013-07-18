using System;
using Pierce.Net;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace Pierce.Net.Example
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var queue = new RequestQueue();

            var google = new Uri("http://www.google.com");
            var yahoo = new Uri("http://www.yahoo.com");

            for (var x = 1; x <= 16; x++)
            {
                Thread.Sleep(200);

                var request_number = x;
                var uri = x % 2 == 0 ?
                    google :
                    yahoo;

                queue.Add(new Request
                {
                    Uri = uri,
                    OnResponse = response =>
                    {
                        var @string = Encoding.UTF8.GetString(response.Data).Substring(0, 64);
                        //Console.WriteLine("{0}: {1}", request_number, @string);
                        Console.WriteLine("received response {0}", request_number);
                    },
                });
            };

            Console.ReadLine();
        }
    }
}
