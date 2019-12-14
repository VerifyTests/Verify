using System;

class DisposableVerifier:
    Verifier,
    IDisposable
{
    public DisposableVerifier(Type testType, string directory, string testName) :
        base(testType, directory, testName)
    {
        CounterContext.Start();
    }

    public void Dispose()
    {
        CounterContext.Stop();
    }
}