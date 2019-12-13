using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Verify;

namespace VerifyXunit
{
    public partial class VerifyBase
    {
        #region VerifyBinary
        public Task VerifyBinary(Stream input, VerifySettings? settings = null)
            #endregion
        {
            return verifier.VerifyBinary(input, settings);
        }

        public Task VerifyBinary(IEnumerable<Stream> streams, VerifySettings? settings = null)
        {
            return verifier.VerifyBinary(streams, settings);
        }
    }
}