using VerifyTests;

namespace VerifyExpecto
{
    public static partial class Verifier
    {
        public static Task Throws(
            Action target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return Verify(settings, sourceFile, _ => _.Throws(target));
        }

        public static Task Throws(
            Func<object?> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return Verify(settings, sourceFile, _ => _.Throws(target));
        }

        public static Task ThrowsTask(
            Func<Task> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return Verify(settings, sourceFile, _ => _.ThrowsTask(target));
        }

        public static Task ThrowsTask<T>(
            Func<Task<T>> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return Verify(settings, sourceFile, _ => _.ThrowsTask(target));
        }

        public static Task ThrowsValueTask(
            Func<ValueTask> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return Verify(settings, sourceFile, _ => _.ThrowsValueTask(target));
        }

        public static Task ThrowsValueTask<T>(
            Func<ValueTask<T>> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            return Verify(settings, sourceFile, _ => _.ThrowsValueTask(target));
        }
    }
}