using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VerifyXunit
{
    public interface IVerify
    {

    }
    public partial class VerifyBase
    {
        public async Task Verify<T>(Task<T> task)
        {
            var target = await task;
            if (target == null)
            {
                throw new Exception("Task returned null.");
            }

            try
            {
                await Verify(target);
            }
            finally
            {
#if NETSTANDARD2_1
                if (target is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
                else if (target is IDisposable disposable)
                {
                    disposable.Dispose();
                }
#else
                if (target is IDisposable disposable)
                {
                    disposable.Dispose();
                }
#endif
            }
        }

        public Task Verify(object target)
        {
            Guard.AgainstNull(target, nameof(target));
            return Verify(target, serialization.currentSettings);
        }

        public Task Verify(object target, JsonSerializerSettings jsonSerializerSettings)
        {
            Guard.AgainstNull(target, nameof(target));
            Guard.AgainstNull(jsonSerializerSettings, nameof(jsonSerializerSettings));
            var formatJson = JsonFormatter.AsJson(target, jsonSerializerSettings);
            return Verify(formatJson);
        }

        public JsonSerializerSettings BuildJsonSerializerSettings()
        {
            return serialization.BuildSettings();
        }
    }
}