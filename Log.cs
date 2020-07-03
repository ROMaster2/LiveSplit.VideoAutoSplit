﻿using System;
using System.Diagnostics;
using System.IO;

namespace LiveSplit.VAS
{
    public static class Log
    {
        private const string PREFIX = "[VAS] ";
        private const string STANDARD_FORMAT = "{0} {1}";

        private static TextWriter _TextWriter = new StringWriter();

        public static bool VerboseEnabled { get; set; } = true;

        public static bool WriteToFileEnabled { get; set; } = true;

        public static event EventHandler<string> LogUpdated;

        static Log()
        {
            try
            {
                if (!EventLog.SourceExists("VideoAutoSplit"))
                    EventLog.CreateEventSource("VideoAutoSplit", "Application");

                var listener = new EventLogTraceListener("VideoAutoSplit");
                listener.Filter = new EventTypeFilter(SourceLevels.Warning);
                Trace.Listeners.Add(listener);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        public static string ReadAll() => _TextWriter.ToString();

        private static void Write(string message)
        {
            try
            {
                var str = "[" + DateTime.Now.ToString("hh:mm:ss.fff") + "] " + message;
                _TextWriter.WriteLine(str);
                LogUpdated?.Invoke(null, str);
                if (WriteToFileEnabled) WriteToFile(str);
            }
            catch { }
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
            try
            {
                Trace.TraceInformation(STANDARD_FORMAT, PREFIX, message);
                Write(message);
            }
            catch { }
        }

        public static void Warning(string message)
        {
            try
            {
                Trace.TraceWarning(STANDARD_FORMAT, PREFIX, message);
                Write(message);
            }
            catch { }
        }

        public static void Error(Exception ex, string description)
        {
            try
            {
                Trace.TraceError(STANDARD_FORMAT, PREFIX, description);
                Write(description);
                Trace.TraceError("{0}\n\n{1}", ex.Message, ex.StackTrace);
                Write(ex.Message);
                Write(ex.StackTrace);
            }
            catch { }
        }

        public static void Flush()
        {
            try
            {
                _TextWriter.Flush();
                _TextWriter = new StringWriter();
            }
            catch { }
        }
    }
}
