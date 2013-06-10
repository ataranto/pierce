using System;
using System.Threading;

namespace Pierce.Example.Models
{
    public abstract class DateTimeModel : IDateTimeModel
    {
        public event EventHandler Changed;
        public abstract DateTime Value { get; }

        public DateTimeModel()
        {
            new Timer(timer_Callback, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        private void timer_Callback(Object state)
        {
            var handler = Changed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}

