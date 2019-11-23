using System.Threading.Tasks;
using Xunit;

namespace VerifyXunit
{
    public partial class VerifyBase :
        XunitContextBase
    {
        public Task Verify(object target)
        {
            Guard.AgainstNull(target, nameof(target));
            var formatJson = JsonFormatter.AsJson(target);
            return VerifyText(formatJson, ".json");
        }
    }
}