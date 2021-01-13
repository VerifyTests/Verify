using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VerifyTests
{
    partial class InnerVerifier
    {
        public async Task Verify<T>(IAsyncEnumerable<T> target)
        {
            Guard.AgainstNull(target, nameof(target));
            List<T> list = new();
            await foreach (var item in target)
            {
                list.Add(item);
            }

            try
            {
                await VerifyBinary(Enumerable.Empty<ConversionStream>(), list, null);
            }
            finally
            {
                foreach (var item in list)
                {
                    await DoDispose(item);
                }
            }
        }
    }
}