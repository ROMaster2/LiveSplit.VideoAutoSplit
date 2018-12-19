using System;
using System.Diagnostics;
using System.IO;

namespace LiveSplit.VAS
{
    public static class Log
    {
        private static TextWriter _TextWriter = new StringWriter();
        public static event EventHandler<string> LogUpdated;

        static Log()
        {
            try
            {
                if (!EventLog.SourceExists("VideoAutoSplitter"))
                    EventLog.CreateEventSource("VideoAutoSplitter", "Application");
            }
            catch { }

            try
            {
                var listener = new EventLogTraceListener("VideoAutoSplitter");
                listener.Filter = new EventTypeFilter(SourceLevels.Warning);
                Trace.Listeners.Add(listener);
            }
            catch { }
        }

        public static string ReadAll()
        {
            return _TextWriter.ToString();
        }

        private static void Write(string message)
        {
            var str = "[" + DateTime.Now.ToString("hh:mm:ss.fff") + "] " + message;
            _TextWriter.WriteLine(str);
            if (LogUpdated != null) LogUpdated(null, str);
        }

        public static void Info(string message)
        {
            try
            {
                Trace.TraceInformation(message);
                Write(message);
            }
            catch { }
        }

        public static void Warning(string message)
        {
            try
            {
                Trace.TraceWarning(message);
                Write(message);
            }
            catch { }
        }

        public static void Error(string message)
        {
            try
            {
                Trace.TraceError(message);
                Write(message);
            }
            catch { }
        }

        public static void Error(Exception ex)
        {
            try
            {
                Trace.TraceError("{0}\n\n{1}", ex.Message, ex.StackTrace);
                Write(ex.Message);
                Write(ex.StackTrace);
            }
            catch { }
        }

        public static void Flush()
        {
            _TextWriter.Flush();
            _TextWriter = new StringWriter();
        }
    }
}
