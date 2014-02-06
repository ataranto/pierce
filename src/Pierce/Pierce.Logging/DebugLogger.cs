using System;

namespace Pierce.Logging
{
	public class DebugLogger : ILogger
    {
        public DebugLogger(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public void Debug(string format, params object[] args)
        {
			System.Diagnostics.Debug.
				WriteLine(Name + ":Debug:" + String.Format(format, args));
        }

        public void Error(string format, params object[] args)
        {
            Error(null, format, args);
        }

        public void Error(Exception exception, string format, params object[] args)
        {
            var exception_string = exception == null ?
                null :
                "\n" + exception;
			System.Diagnostics.Debug.
				WriteLine(Name + ":Error:" + String.Format(format, args) + exception_string);
        }

        public void Fatal(Exception exception, string format, params object[] args)
        {
            var exception_string = exception == null ?
                null :
                "\n" + exception;
            System.Diagnostics.Debug.
                WriteLine(Name + ":Fatal:" + String.Format(format, args) + exception_string);
        }
    }
}

