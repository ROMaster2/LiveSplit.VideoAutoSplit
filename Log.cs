using System;
using System.Diagnostics;
using System.IO;

namespace LiveSplit.VAS
{
    public static class Log
    {
        private const string PREFIX = "[VAS] ";
        private const string STANDARD_FORMAT = "{0} {1}";

        private static TextWriter _TextWriter = new StringWriter();

#if DEBUG
        public static bool VerboseEnabled { get; set; } = true;
#else
        public static bool VerboseEnabled { get; set; } = false;
#endif

        public static bool WriteToFileEnabled { get; set; } = true;

        public static event EventHandler<string> LogUpdated;

        static Log()
        {
            if (!EventLog.SourceExists("VideoAutoSplit"))
                EventLog.CreateEventSource("VideoAutoSplit", "Application");
            var listener = new EventLogTraceListener("VideoAutoSplit");
            listener.Filter = new EventTypeFilter(SourceLevels.Warning);
            Trace.Listeners.Add(listener);
        }

        public static string ReadAll() => _TextWriter.ToString();

        private static void Write(string message)
        {
            var str = "[" + DateTime.Now.ToString("hh:mm:ss.fff") + "] " + message;
            _TextWriter.WriteLine(str);
            LogUpdated?.Invoke(null, str);
            if (WriteToFileEnabled) WriteToFile(str);
        }

        private static void WriteToFile(string message)
        {
            try
            {
                using (var fs = new FileStream(@"VASErrorLog.txt", FileMode.Append, FileAccess.Write))
                using (var sw = new StreamWriter(fs))
                {
                    sw.WriteLineAsync(message);
                }
            }
            catch { }
        }

        public static void Verbose(string message)
        {
            if (VerboseEnabled) Info(message);
        }

        public static void Info(string message)
        {
            Trace.TraceInformation(STANDARD_FORMAT, PREFIX, message);
            Write(message);
        }

        public static void Warning(string message)
        {
            Trace.TraceWarning(STANDARD_FORMAT, PREFIX, message);
            Write(message);
        }

        public static void Error(Exception ex, string description)
        {
            Trace.TraceError(STANDARD_FORMAT, PREFIX, description);
            Write(description);
            Trace.TraceError("{0}\n\n{1}", ex.Message, ex.StackTrace);
            Write(ex.Message);
            Write(ex.StackTrace);
        }

        public static void Flush()
        {
            _TextWriter.Flush();
            _TextWriter = new StringWriter();
        }
    }
}
