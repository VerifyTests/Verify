using System;
using System.Threading.Tasks;
using VerifyTests;

namespace VerifyNUnit
{
    public partial class VerifyBase
    {
        public Task Throws(
            Action target,
            VerifySettings? settings = null)
        {
            settings ??= this.settings;
            return Verifier.Throws(target, settings, sourceFile);
        }

        public Task Throws(
            Func<object> target,
            VerifySettings? settings = null)
        {
            settings ??= this.settings;
            return Verifier.Throws(target, settings, sourceFile);
        }

        public Task ThrowsAsync(
            Func<Task> target,
            VerifySettings? settings = null)
        {
            settings ??= this.settings;
            return Verifier.ThrowsAsync(target, settings, sourceFile);
        }

        public Task ThrowsAsync(
            Func<ValueTask> target,
            VerifySettings? settings = null)
        {
            settings ??= this.settings;
            return Verifier.ThrowsAsync(target, settings, sourceFile);
        }
    }
}