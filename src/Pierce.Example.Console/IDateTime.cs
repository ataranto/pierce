using System;

namespace Pierce.Example.Console
{
    public interface IDateTime
    {
        event EventHandler Changed;
        System.DateTime Value { get; }
    }
}

