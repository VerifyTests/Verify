using System.Threading.Tasks;
using Newtonsoft.Json;
using Verify;

namespace VerifyXunit
{
    public partial class VerifyBase
    {
        public Task Verify<T>(Task<T> task, VerifySettings? settings = null)
        {
            return verifier.Verify(task,settings);
        }

        public Task Verify(object target, VerifySettings? settings = null)
        {
            return verifier.Verify(target,settings);
        }

        public Task Verify(object target, JsonSerializerSettings jsonSerializerSettings, VerifySettings? settings = null)
        {
            return verifier.Verify(target, jsonSerializerSettings,settings);
        }

        public JsonSerializerSettings BuildJsonSerializerSettings()
        {
            return verifier.BuildJsonSerializerSettings();
        }
    }
}