using System;
using System.Threading.Tasks;

namespace VerifyTests
{
    partial class InnerVerifier
    {
        public Task Throws(Action target)
        {
            try
            {
                target();
            }
            catch (Exception exception)
            {
                return Verify(exception);
            }

            throw new("Did not throw.");
        }

        public Task Throws(Func<object?> target)
        {
            try
            {
                target();
            }
            catch (Exception exception)
            {
                return Verify(exception);
            }

            throw new("Did not throw.");
        }

        public async Task ThrowsValueTask(Func<ValueTask> target)
        {
            try
            {
                await target();
            }
            catch (Exception exception)
            {
                await Verify(exception);
                return;
            }

            throw new("Did not throw.");
        }

        public async Task ThrowsValueTask<T>(Func<ValueTask<T>> target)
        {
            try
            {
                await target();
            }
            catch (Exception exception)
            {
                await Verify(exception);
                return;
            }

            throw new("Did not throw.");
        }

        public async Task ThrowsTask(Func<Task> target)
        {
            try
            {
                await target();
            }
            catch (Exception exception)
            {
                await Verify(exception);
                return;
            }

            throw new("Did not throw.");
        }

        public async Task ThrowsTask<T>(Func<Task<T>> target)
        {
            try
            {
                await target();
            }
            catch (Exception exception)
            {
                await Verify(exception);
                return;
            }

            throw new("Did not throw.");
        }
    }
}