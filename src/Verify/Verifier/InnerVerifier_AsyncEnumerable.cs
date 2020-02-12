using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Verify;

partial class InnerVerifier
{
    public async Task Verify<T>(IAsyncEnumerable<T> enumerable, VerifySettings? settings = null)
    {
        Guard.AgainstNull(enumerable, nameof(enumerable));
        settings = settings.OrDefault();
        var list = new List<T>();
        await foreach (var item in enumerable)
        {
            list.Add(item);
        }

        try
        {
            await Verify(list, settings);
        }
        finally
        {
            foreach (var item in list)
            {
                if (item is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
                else if (item is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}