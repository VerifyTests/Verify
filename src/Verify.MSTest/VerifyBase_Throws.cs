using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VerifyTests;

namespace VerifyMSTest
{
    public partial class VerifyBase
    {
        public SettingsTask Throws(
            Action target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return Verify(settings, sourceFile, _ => _.Throws(target));
        }

        public SettingsTask Throws(
            Func<object?> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return Verify(settings, sourceFile, _ => _.Throws(target));
        }

        public SettingsTask ThrowsAsync(
            Func<Task> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return Verify(settings, sourceFile, _ => _.ThrowsAsync(target));
        }

        public SettingsTask ThrowsAsync(
            Func<ValueTask> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return Verify(settings, sourceFile, _ => _.ThrowsAsync(target));
        }
    }
}