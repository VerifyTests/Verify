using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VerifyXunit
{
    public partial class VerifyBase
    {
        public Task Verify<T>(Task<T> task)
        {
            return verifier.Verify(task);
        }

        public Task Verify(object target)
        {
            return verifier.Verify(target);
        }

        public Task Verify(object target, JsonSerializerSettings jsonSerializerSettings)
        {
            return verifier.Verify(target, jsonSerializerSettings);
        }

        public JsonSerializerSettings BuildJsonSerializerSettings()
        {
            return verifier.BuildJsonSerializerSettings();
        }
    }
}