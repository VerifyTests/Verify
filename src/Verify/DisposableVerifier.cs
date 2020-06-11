using System;

class DisposableVerifier:
    InnerVerifier,
    IDisposable
{
    public DisposableVerifier(Type testType, string testName, string sourceFile) :
        base(testType, testName, sourceFile)
    {
        CounterContext.Start();
    }

    public void Dispose()
    {
        CounterContext.Stop();
    }
}