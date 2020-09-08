using System;
using System.Threading.Tasks;

namespace VerifyTests
{
    partial class InnerVerifier
    {
        public async Task Verify<T>(Task<T> task, VerifySettings settings)
        {
            Guard.AgainstNull(task, nameof(task));
            var target = await task;

            try
            {
                await Verify(target, settings);
            }
            finally
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
        }
    }
}