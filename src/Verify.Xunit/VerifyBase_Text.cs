using System.Threading.Tasks;

namespace VerifyXunit
{
    public partial class VerifyBase
    {
        public Task Verify(string target)
        {
            Guard.AgainstNull(target, nameof(target));
            return Verify(target, textExtension);
        }
    }
}