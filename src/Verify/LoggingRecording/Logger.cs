using System;
using Microsoft.Extensions.Logging;
using VerifyTests;

class Logger :
    ILogger
{
    string? category;
    LogLevel level;
    LoggerProvider provider;

    public Logger(string? category, LogLevel level, LoggerProvider provider)
    {
        this.category = category;
        this.level = level;
        this.provider = provider;
    }

    public void Log<TState>(LogLevel level, EventId eventId, TState state, Exception? exception, Func<TState?, Exception?, string> formatter)
    {
        provider.AddEntry(level, category, eventId, state, exception, formatter);
    }

    public bool IsEnabled(LogLevel level)
    {
        return level >= this.level;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        provider.StartScope(state);
        return new LoggerScope(() => provider.EndScope());
    }
}

class Logger<T> :
    Logger,
    ILogger<T>
{
    public Logger(LogLevel level, LoggerProvider provider) :
        base(typeof(T).Name, level, provider)
    {
    }
}