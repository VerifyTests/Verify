partial class InnerVerifier
{
    public async Task<VerifyResult> Verify<T>(Task<T> task)
        where T : notnull
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

    public async Task<VerifyResult> Verify<T>(IAsyncEnumerable<T> target)
        where T : notnull
    {
        var list = await target.ToList();

        try
        {
            return await VerifyInner(list, null, emptyTargets);
        }
        finally
        {
            foreach (var item in list)
            {
                await DoDispose(item);
            }
        }
    }

    static async Task DoDispose<T>(T target)
        where T : notnull
    {
        if (target is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
        else if (target is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    public async Task<VerifyResult> Verify<T>(ValueTask<T> task)
        where T : notnull
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