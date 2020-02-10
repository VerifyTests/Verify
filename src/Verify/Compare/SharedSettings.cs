using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Verify
{
    public static partial class SharedVerifySettings
    {
        static Dictionary<string, Func<Stream, Stream, Task<bool>>> comparers = new Dictionary<string, Func<Stream, Stream, Task<bool>>>();

        internal static bool TryGetComparer(string extension, out Func<Stream, Stream, Task<bool>> comparer)
        {
            return comparers.TryGetValue(extension, out comparer);
        }

        public static void RegisterComparer(
            string extension,
            Func<Stream, Stream, bool> func)
        {
            Guard.AgainstNull(func, nameof(func));
            RegisterComparer(
                extension,
                (stream, settings) => Task.FromResult(func(stream, settings)));
        }

        public static void RegisterComparer(
            string extension,
            Func<Stream, Stream, Task<bool>> func)
        {
            Guard.AgainstNull(func, nameof(func));
            Guard.AgainstBadExtension(extension, nameof(extension));
            comparers[extension] = func;
        }
    }
}