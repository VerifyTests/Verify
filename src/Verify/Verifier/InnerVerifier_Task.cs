namespace VerifyTests;

partial class InnerVerifier
{
    public async Task<VerifyResult> Verify<T>(Task<T> task)
    {
        var target = await task;

        try
        {
            return await Verify(target);
        }
        finally
        {
            await DoDispose(target);
        }
    }

    public async Task<VerifyResult> Verify<T>(IAsyncEnumerable<T?> target)
    {
        var list = await target.ToList();

        try
        {
            return await VerifyInner(list, null, emptyTargets, true);
        }
        finally
        {
            foreach (var item in list)
            {
                await DoDispose(item);
            }
        }
    }

    static async Task DoDispose<T>(T? target)
    {
        switch (target)
        {
            case IAsyncDisposable asyncDisposable:
                await asyncDisposable.DisposeAsync();
                break;
            case IDisposable disposable:
                disposable.Dispose();
                break;
        }
    }

    public async Task<VerifyResult> Verify<T>(ValueTask<T> task)
    {
        var target = await task;

        try
        {
            return await Verify(target);
        }
        finally
        {
            await DoDispose(target);
        }
    }
}