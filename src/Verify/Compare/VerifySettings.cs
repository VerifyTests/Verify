using System;
using System.IO;
using System.Threading.Tasks;

namespace Verify
{
    public partial class VerifySettings
    {
        internal Func<Stream, Stream, Task<bool>>? comparer;

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
            comparer = func;
        }
    }
}