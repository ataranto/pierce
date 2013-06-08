using System;
using MonoMac.AppKit;
using Pierce.Example.Views;

namespace Pierce.Example.Mac.Views
{
    public class TextDateTimeView : NSTextField, IDateTimeView
    {
        public TextDateTimeView()
        {
        }


        public void SetValue(string value)
        {
            StringValue = value;
        }
    }
}

