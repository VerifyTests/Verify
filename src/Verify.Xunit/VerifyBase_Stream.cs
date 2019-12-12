using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace VerifyXunit
{
    public partial class VerifyBase
    {
        #region VerifyBinary
        public Task VerifyBinary(Stream input, string extension = "bin")
            #endregion
        {
            return verifier.VerifyBinary(input, extension);
        }

        public Task VerifyBinary(IEnumerable<Stream> streams, string extension = "bin")
        {
            return verifier.VerifyBinary(streams, extension);
        }
    }
}