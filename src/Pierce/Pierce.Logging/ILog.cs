using System;

namespace Pierce.Logging
{
    public interface ILog
    {
        string Tag { set; }
        void Debug(string format, params object[] args);
        void Error(string format, params object[] args);
        void Error(Exception exception, string format, params object[] args);
    }
}

