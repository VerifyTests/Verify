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

            throw new Exception("Did not throw.");
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

            throw new Exception("Did not throw.");
        }

        public async Task ThrowsAsync(Func<ValueTask> target)
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

            throw new Exception("Did not throw.");
        }

        public async Task ThrowsAsync(Func<Task> target)
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

            throw new Exception("Did not throw.");
        }
    }
}