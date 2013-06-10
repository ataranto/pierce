using System;

namespace Pierce.Example.Models
{
    public class LocalDateTimeModel : DateTimeModel
    {
        public override DateTime Value
        {
            get{ return DateTime.Now; }
        }
    }
}

