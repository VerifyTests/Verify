using System.Threading.Tasks;
using Xunit;

namespace VerifyXunit
{
    public partial class VerifyBase :
        XunitContextBase
    {
        public Task Verify(string target)
        {
            Guard.AgainstNull(target, nameof(target));
            return Verify(target, textExtension);
        }
    }
}