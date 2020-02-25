using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Verify
{
    public static partial class SharedVerifySettings
    {
        static Dictionary<string, Func<VerifySettings, Stream, Stream, Task<bool>>> comparers = new Dictionary<string, Func<VerifySettings, Stream, Stream, Task<bool>>>();

        internal static bool TryGetComparer(string extension, out Func<VerifySettings, Stream, Stream, Task<bool>> comparer)
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
                (settings, stream1, stream2) => Task.FromResult(func(stream1, stream2)));
        }

        public static void RegisterComparer(
            string extension,
            Func<VerifySettings, Stream, Stream, bool> func)
        {
            Guard.AgainstNull(func, nameof(func));
            RegisterComparer(
                extension,
                (settings, stream1, stream2) => Task.FromResult(func(settings, stream1, stream2)));
        }

        public static void RegisterComparer(
            string extension,
            Func<Stream, Stream, Task<bool>> func)
        {
            Guard.AgainstNull(func, nameof(func));
            RegisterComparer(
                extension,
                (settings, stream1, stream2) => func(stream1, stream2));
        }

        public static void RegisterComparer(
            string extension,
            Func<VerifySettings, Stream, Stream, Task<bool>> func)
        {
            Guard.AgainstNull(func, nameof(func));
            Guard.AgainstBadExtension(extension, nameof(extension));
            comparers[extension] = func;
        }
    }
}