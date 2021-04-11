using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VerifyTests;

namespace VerifyNUnit
{
    public static partial class Verifier
    {
        public static SettingsTask Verify<T>(
            Task<T> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return Verify(settings, sourceFile, _ => _.Verify(target));
        }

        public static SettingsTask Verify<T>(
            ValueTask<T> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return Verify(settings, sourceFile, _ => _.Verify(target));
        }

        public static SettingsTask Verify<T>(
            IAsyncEnumerable<T> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return Verify(settings, sourceFile, _ => _.Verify(target));
        }

        public static SettingsTask Verify<T>(
            T target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return Verify(settings, sourceFile, _ => _.Verify(target));
        }

        public static SettingsTask VerifyJson(
            string target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return Verify(settings, sourceFile, _ => _.VerifyJson(target));
        }

        public static SettingsTask VerifyJson(
            JToken target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return Verify(settings, sourceFile, _ => _.VerifyJson(target));
        }

        public static SettingsTask VerifyJson(
            Stream target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return Verify(settings, sourceFile, _ => _.VerifyJson(target));
        }
    }
}