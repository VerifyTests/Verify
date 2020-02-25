using System;
using System.IO;
using System.Threading.Tasks;

namespace Verify
{
    public partial class VerifySettings
    {
        internal Func<VerifySettings, Stream, Stream, Task<bool>>? comparer;

        public void UseComparer(
            Func<Stream, Stream, bool> func)
        {
            Guard.AgainstNull(func, nameof(func));
            UseComparer(
                (stream, settings) => Task.FromResult(func(stream, settings)));
        }

        public void UseComparer(
            Func<Stream, Stream, Task<bool>> func)
        {
            Guard.AgainstNull(func, nameof(func));
            comparer = (settings, stream1, stream2) => func(stream1, stream2);
        }
    }
}