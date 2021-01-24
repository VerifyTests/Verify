using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VerifyTests;

namespace VerifyXunit
{
    public static partial class Verifier
    {
        public static SettingsTask Throws(
            Action target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return Verify(settings, sourceFile, _ => _.Throws(target));
        }

        public static SettingsTask Throws(
            Func<object?> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return Verify(settings, sourceFile, _ => _.Throws(target));
        }

        public static SettingsTask ThrowsAsync(
            Func<Task> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return Verify(settings, sourceFile, _ => _.ThrowsAsync(target));
        }

        public static SettingsTask ThrowsAsync<T>(
            Func<Task<T>> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return Verify(settings, sourceFile, _ => _.ThrowsAsync(target));
        }

        public static SettingsTask ThrowsAsync(
            Func<ValueTask> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return Verify(settings, sourceFile, _ => _.ThrowsAsync(target));
        }

        public static SettingsTask ThrowsAsync<T>(
            Func<ValueTask<T>> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return Verify(settings, sourceFile, _ => _.ThrowsAsync(target));
        }
    }
}