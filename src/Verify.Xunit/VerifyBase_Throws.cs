using System;
using System.Threading.Tasks;
using VerifyTests;

namespace VerifyXunit
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

        public Task Throws(
            Func<Task> target,
            VerifySettings? settings = null)
        {
            settings ??= this.settings;
            return Verifier.ThrowsAsync(target, settings, sourceFile);
        }

        public Task Throws(
            Func<ValueTask> target,
            VerifySettings? settings = null)
        {
            settings ??= this.settings;
            return Verifier.ThrowsAsync(target, settings, sourceFile);
        }
    }
}