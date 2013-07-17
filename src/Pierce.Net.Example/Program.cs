using System;
using Pierce.Net;
using System.Reactive.Linq;
using System.Text;
using System.Reactive.Subjects;
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

            var uri = new Uri("http://www.google.com");

            for (var x = 0; x < 8; x++)
            {
                Thread.Sleep(500);
                var request = new Request { Uri = uri };
                var request_number = x;

                queue.
                    Add(request).
                    Subscribe(response =>
                {
                    var @string = Encoding.UTF8.GetString(response.Data).Substring(0, 64);
                    Console.WriteLine("{0}: {1}", request_number, @string);
                });
            };

            Console.ReadLine();
        }
    }
}
