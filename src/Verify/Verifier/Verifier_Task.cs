using System;
using System.Threading.Tasks;
using Verify;

partial class Verifier
{
    public async Task Verify<T>(Task<T> task, VerifySettings? settings = null)
    {
        Guard.AgainstNull(task, nameof(task));
        settings = settings.OrDefault();
        var target = await task;
        if (target == null)
        {
            throw new Exception("Task returned null.");
        }

        try
        {
            await Verify(target, settings);
        }
        finally
        {
#if NETSTANDARD2_1
            if (target is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else if (target is IDisposable disposable)
            {
                disposable.Dispose();
            }
#else
            if (target is IDisposable disposable)
            {
                disposable.Dispose();
            }
#endif
        }
    }
}