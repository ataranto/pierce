using System;

namespace Pierce.Example.Models
{
    public class UtcDateTimeModel : DateTimeModel
    {
        public override DateTime Value
        {
            get { return DateTime.UtcNow; }
        }
    }
}

