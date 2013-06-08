using System;

namespace Pierce.Example.Models
{
    public interface IDateTimeModel
    {
        event EventHandler Changed;
        DateTime Value { get; }
    }
}

