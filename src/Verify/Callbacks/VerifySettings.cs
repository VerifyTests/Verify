using System;
using System.Threading.Tasks;

namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        static FirstVerify? handleOnFirstVerify;

        public static void OnFirstVerify(FirstVerify firstVerify)
        {
            handleOnFirstVerify += firstVerify;
        }

        internal static Task RunOnFirstVerify(FilePair item)
        {
            if (handleOnFirstVerify == null)
            {
                return Task.CompletedTask;
            }

            return handleOnFirstVerify(item);
        }

        static VerifyMismatch? handleOnVerifyMismatch;

        internal static Task RunOnVerifyMismatch(FilePair item, string? message)
        {
            if (handleOnVerifyMismatch == null)
            {
                return Task.CompletedTask;
            }

            return handleOnVerifyMismatch(item, message);
        }

        public static void OnVerifyMismatch(VerifyMismatch verifyMismatch)
        {
            handleOnVerifyMismatch += verifyMismatch;
        }

        public static void OnVerify(Action before, Action after)
        {
            beforeVerify += before;
            afterVerify += after;
        }

        static Action? beforeVerify;

        internal static void RunBeforeCallbacks()
        {
            beforeVerify?.Invoke();
        }

        static Action? afterVerify;

        internal static void RunAfterCallbacks()
        {
            afterVerify?.Invoke();
        }
    }
}