using System;
using MonoMac.AppKit;
using Pierce.Example.Views;

namespace Pierce.Example.Mac.Views
{
    public class DigitalDateTimeView : NSTextField, IDateTimeView
    {
        public DigitalDateTimeView()
        {
            Editable = false;
        }

        public void SetValue(DateTime value)
        {
            StringValue = value.ToString();
        }
    }
}

