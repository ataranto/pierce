using System;

namespace Pierce.Logging
{
    public interface ILogger
    {
        string Name { get; }

        void Debug(string format, params object[] args);
        void Error(string format, params object[] args);
        void Error(Exception exception, string format, params object[] args);
        void Fatal(Exception exception, string format, params object[] args);
    }
}

