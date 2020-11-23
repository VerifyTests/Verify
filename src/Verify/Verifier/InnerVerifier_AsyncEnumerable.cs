using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VerifyTests
{
    partial class InnerVerifier
    {
        public async Task Verify<T>(IAsyncEnumerable<T> target)
        {
            Guard.AgainstNull(target, nameof(target));
            List<T> list = new();
            await foreach (var item in target)
            {
                list.Add(item);
            }

            try
            {
                await SerializeAndVerify(list, VerifierSettings.GetJsonAppenders(settings));
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