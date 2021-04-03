using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace VerifyTests
{
    public class LoggerProvider :
        ILoggerProvider,
        ILogger
    {
        LogLevel level;
        internal ConcurrentQueue<object> entries = new();
        Logger defaultLogger;

        public LoggerProvider(LogLevel level)
        {
            this.level = level;
            defaultLogger = new Logger(null, level, this);
        }

        public void Dispose()
        {
        }

        public ILogger CreateLogger(string category)
        {
            return new Logger(category, level, this);
        }

        public ILogger<T> CreateLogger<T>()
        {
            return new Logger<T>(level, this);
        }

        internal void AddEntry<TState>(LogLevel? level, string? category, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (level != null && !(level >= this.level)) return;
            var entry = new LogItem(level, category, eventId, exception, formatter.Invoke(state,exception));
            entries.Enqueue(entry);
        }

        public void Log<TState>(LogLevel level, EventId eventId, TState state, Exception exception, Func<TState, Exception?, string> formatter)
        {
            defaultLogger.Log(level, eventId, state, exception, formatter);
        }

        public bool IsEnabled(LogLevel level)
        {
            return defaultLogger.IsEnabled(level);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return defaultLogger.BeginScope(state);
        }
    }
}