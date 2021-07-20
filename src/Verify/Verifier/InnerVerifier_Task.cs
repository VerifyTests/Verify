using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VerifyTests
{
    partial class InnerVerifier
    {
        public async Task Verify<T>(Task<T> task)
        {
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
            var list = new List<T>();
            await foreach (var item in target)
            {
                list.Add(item);
            }

            try
            {
                await VerifyInner(list, null, emptyTargets);
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