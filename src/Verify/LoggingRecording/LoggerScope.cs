using System;

class LoggerScope :
    IDisposable
{
    Action log;

    public LoggerScope(Action log)
    {
        this.log = log;
    }

    public void Dispose()
    {
        log();
    }
}