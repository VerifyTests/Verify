using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VerifyXunit
{
    public interface IVerify
    {

    }
    public partial class VerifyBase
    {
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