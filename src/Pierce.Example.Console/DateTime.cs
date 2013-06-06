using System;
using System.Threading;

namespace Pierce.Example.Console
{
    public class DateTime : IDateTime
    {
        public event EventHandler Changed;

        public DateTime()
        {
            new Timer(timer_Callback, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        public System.DateTime Value
        {
            get { return System.DateTime.Now; }
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

