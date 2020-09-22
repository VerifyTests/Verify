using System;
using System.Threading.Tasks;

namespace VerifyTests
{
    partial class InnerVerifier
    {
        public Task Throws(Action target, VerifySettings settings)
        {
            try
            {
                target();
            }
            catch (Exception exception)
            {
                return Verify(exception, settings);
            }

            throw new Exception("Did not throw.");
        }

        public Task Throws(Func<object> target, VerifySettings settings)
        {
            try
            {
                target();
            }
            catch (Exception exception)
            {
                return Verify(exception, settings);
            }

            throw new Exception("Did not throw.");
        }

        public async Task ThrowsAsync(Func<ValueTask> target, VerifySettings settings)
        {
            try
            {
                await target();
            }
            catch (Exception exception)
            {
                await Verify(exception, settings);
            }

            throw new Exception("Did not throw.");
        }

        public async Task ThrowsAsync(Func<Task> target, VerifySettings settings)
        {
            try
            {
                await target();
            }
            catch (Exception exception)
            {
                await Verify(exception, settings);
            }

            throw new Exception("Did not throw.");
        }
    }
}