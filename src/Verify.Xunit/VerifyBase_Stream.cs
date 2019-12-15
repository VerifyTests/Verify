using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Verify;

namespace VerifyXunit
{
    #region VerifyBinaryXunit
    public partial class VerifyBase
    {
        public Task VerifyBinary(
            Stream input,
            VerifySettings? settings = null)
        {
            return verifier.VerifyBinary(input, settings);
        }

        public Task VerifyBinary<T>(
            T input,
            VerifySettings? settings = null)
        {
            if (input is Stream stream)
            {
                return verifier.VerifyBinary(stream, settings);
            }
            return verifier.VerifyBinary(input, settings);
        }

        public Task VerifyBinary(
            IEnumerable<Stream> streams,
            VerifySettings? settings = null)
        {
            return verifier.VerifyBinary(streams, settings);
        }
    }
    #endregion
}