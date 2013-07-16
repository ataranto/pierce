using System;
using Pierce.Net;
using System.Reactive.Linq;
using System.Text;

namespace Pierce.Net.Example
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var queue = new RequestQueue();

            var uri = new Uri("http://www.google.com");
            var request = new Request
            {
                Uri = uri,
            };

            queue.
                Add(request).
                Subscribe(response =>
            {
                var @string = Encoding.UTF8.GetString(response.Data);
                Console.WriteLine(@string);
            });

            Console.ReadLine();
        }
    }
}
