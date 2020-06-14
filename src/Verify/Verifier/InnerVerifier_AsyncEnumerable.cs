using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VerifyTesting
{
    partial class InnerVerifier
    {
        public async Task Verify<T>(IAsyncEnumerable<T> target, VerifySettings? settings = null)
        {
            Guard.AgainstNull(target, nameof(target));
            settings = settings.OrDefault();
            var list = new List<T>();
            await foreach (var item in target)
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
}