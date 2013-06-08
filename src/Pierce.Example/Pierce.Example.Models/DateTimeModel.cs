using System;
using System.Threading;

namespace Pierce.Example.Models
{
    public class DateTimeModel : IDateTimeModel
    {
        public event EventHandler Changed;

        public DateTimeModel()
        {
            new Timer(timer_Callback, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        public DateTime Value
        {
            get { return DateTime.Now; }
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

