using System;

namespace Pierce.Logging
{
	public class DebugLogger : ILogger
    {
        public string Tag
        {
            private get; set;
        }

        public void Debug(string format, params object[] args)
        {
			System.Diagnostics.Debug.
				WriteLine(Tag + ":Debug:" + String.Format(format, args));
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
				WriteLine(Tag + ":Error:" + String.Format(format, args) + exception_string);
        }
    }
}

