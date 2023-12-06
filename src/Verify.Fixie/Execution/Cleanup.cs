namespace VerifyFixie;

class Cleanup(Action func) :
    IDisposable
{
    public void Dispose() =>
        func();
}