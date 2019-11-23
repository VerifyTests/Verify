using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace VerifyXunit
{
    public partial class VerifyBase :
        XunitContextBase
    {
        public Task Verify(object target)
        {
            Guard.AgainstNull(target, nameof(target));
            var jsonSerializerSettings = BuildJsonSerializerSettings();
            return Verify(target, jsonSerializerSettings);
        }

        public Task Verify(object target, JsonSerializerSettings jsonSerializerSettings)
        {
            Guard.AgainstNull(target, nameof(target));
            Guard.AgainstNull(jsonSerializerSettings, nameof(jsonSerializerSettings));
            var formatJson = JsonFormatter.AsJson(target, jsonSerializerSettings);
            return VerifyText(formatJson);
        }

        public JsonSerializerSettings BuildJsonSerializerSettings()
        {
            return serializationSettings.BuildSettings();
        }
    }
}