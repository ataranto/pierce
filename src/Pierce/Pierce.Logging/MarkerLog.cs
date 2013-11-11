using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;

namespace Pierce.Logging
{
    public class MarkerLog
    {
        private class Marker
        {
            public string Name { get; set; }
            public int ThreadId { get; set; }
            public long Time { get; set; }
        }

        private readonly List<Marker> _markers = new List<Marker>();

        public void Add(string name)
        {
            var marker = new Marker
            {
                Name = name,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                Time = DateTime.Now.Ticks,
            };

            lock (_markers)
            {
                _markers.Add(marker);
            }
        }

        public void Finish(ILogger log, string header)
        {
            if (log == null)
            {
                return;
            }

            lock (_markers)
            {
                var duration = GetDuration();

                var string_builder = new StringBuilder();
                string_builder.AppendFormat(@"({0:ss\.ffff} seconds) {1}", duration, header);
                // XXX check log duration threshold (once we have one)

                var previous_time = _markers.First().Time;
                foreach (var marker in _markers)
                {
                    duration = new TimeSpan(marker.Time - previous_time);
                    previous_time = marker.Time;
                    string_builder.AppendLine().AppendFormat("  {0:ss\\.ffff} [{1:00}] {2}",
                        duration, marker.ThreadId, marker.Name);
                };

                log.Debug(string_builder.ToString());
            }
        }

        private TimeSpan GetDuration()
        {
            if (_markers.Count == 0)
            {
                return TimeSpan.Zero;
            }

            var first = _markers.First().Time;
            var last = _markers.Last().Time;

            return new TimeSpan(last - first);
        }
    }
}

