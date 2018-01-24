using System;
using System.IO;

namespace Bitfinex.Logging
{
    public class Logger
    {
        public TextWriter TextWriter { get; internal set; } = new TraceTextWriter();
        public LogVerbosity Level { get; internal set; } = LogVerbosity.Warning;

        public void Write(LogVerbosity logType, string message)
        {
            if ((int)logType >= (int)Level)
                TextWriter.WriteLine($"{DateTime.Now:hh:mm:ss:fff} | {logType} | {message}");
        }

        public void Info(string message) => Write(LogVerbosity.Info, message);
        public void Info(string message, params object[] args) => Info(String.Format(message, args));
        public void Warning(string message) => Write(LogVerbosity.Warning, message);
        public void Error(string message) => Write(LogVerbosity.Error, message);
        public void Error(Exception exception) => Error(FormatExceptionText(exception));

        private static string FormatExceptionText(Exception exception)
        {
            return $"EXCEPTION [{exception.GetType()}]: {exception.ToString()}";
        }
    }

    public enum LogVerbosity
    {
        Info,
        Warning,
        Error
    }
}
