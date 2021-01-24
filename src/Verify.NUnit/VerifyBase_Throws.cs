using System;
using System.Threading.Tasks;
using VerifyTests;

namespace VerifyNUnit
{
    public partial class VerifyBase
    {
        public SettingsTask Throws(
            Action target,
            VerifySettings? settings = null)
        {
            settings ??= this.settings;
            return Verifier.Throws(target, settings, sourceFile);
        }

        public SettingsTask Throws(
            Func<object?> target,
            VerifySettings? settings = null)
        {
            settings ??= this.settings;
            return Verifier.Throws(target, settings, sourceFile);
        }

        public SettingsTask ThrowsAsync(
            Func<Task> target,
            VerifySettings? settings = null)
        {
            settings ??= this.settings;
            return Verifier.ThrowsAsync(target, settings, sourceFile);
        }

        public SettingsTask ThrowsAsync<T>(
            Func<Task<T>> target,
            VerifySettings? settings = null)
        {
            settings ??= this.settings;
            return Verifier.ThrowsAsync(target, settings, sourceFile);
        }

        public SettingsTask ThrowsAsync(
            Func<ValueTask> target,
            VerifySettings? settings = null)
        {
            settings ??= this.settings;
            return Verifier.ThrowsAsync(target, settings, sourceFile);
        }

        public SettingsTask ThrowsAsync<T>(
            Func<ValueTask<T>> target,
            VerifySettings? settings = null)
        {
            settings ??= this.settings;
            return Verifier.ThrowsAsync(target, settings, sourceFile);
        }
    }
}