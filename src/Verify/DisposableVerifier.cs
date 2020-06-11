using System;

class DisposableVerifier:
    InnerVerifier,
    IDisposable
{
    public DisposableVerifier(string testName, string sourceFile) :
        base(testName, sourceFile)
    {
        CounterContext.Start();
    }

    public void Dispose()
    {
        CounterContext.Stop();
    }
}