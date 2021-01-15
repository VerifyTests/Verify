using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VerifyTests
{
    partial class InnerVerifier
    {
        public async Task Verify<T>(Task<T> task)
        {
            Guard.AgainstNull(task, nameof(task));
            var target = await task;

            try
            {
                await Verify(target);
            }
            finally
            {
                await DoDispose(target);
            }
        }

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
                await VerifyInner(list, null, Enumerable.Empty<ConversionStream>());
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

        public async Task Verify<T>(ValueTask<T> task)
        {
            Guard.AgainstNull(task, nameof(task));
            var target = await task;

            try
            {
                await Verify(target);
            }
            finally
            {
                await DoDispose(target);
            }
        }
    }
}