using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
            defaultLogger = new(null, level, this);
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

        internal void AddEntry<TState>(LogLevel? level, string? category, EventId eventId, TState? state, Exception? exception, Func<TState?, Exception?, string> formatter)
        {
            if (level != null && !(level >= this.level))
            {
                return;
            }

            var message = formatter.Invoke(state, exception);
            if (state is IReadOnlyList<KeyValuePair<string, object>> {Count: 1} dictionary &&
                dictionary.First().Key == "{OriginalFormat}")
            {
                LogItem entry1 = new(level, category, eventId, exception, message, null);
                entries.Enqueue(entry1);
                return;
            }
            LogItem entry = new(level, category, eventId, exception, message, state);
            entries.Enqueue(entry);
        }

        public void Log<TState>(LogLevel level, EventId eventId, TState state, Exception? exception, Func<TState?, Exception?, string> formatter)
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

        internal void EndScope()
        {
            entries.Enqueue(new ScopeEntry("EndScope", null));
        }

        internal void StartScope<TState>(TState state)
        {
            entries.Enqueue(new ScopeEntry("StartScope", state!));
        }
    }
}