#if NET5_0
using System.Collections.Generic;
using System.Diagnostics;

namespace VerifyTests
{
    public static class HttpRecording
    {
        static HttpListener listener;

        static HttpRecording()
        {
            listener = new();
            var subscription = DiagnosticListener.AllListeners.Subscribe(listener);

            VerifierSettings.RegisterJsonAppender(_ =>
            {
                if (!TryFinishRecording(out var entries))
                {
                    return null;
                }

                return new("httpCalls", entries!);
            });
        }

        public static void StartRecording()
        {
            listener.Start();
        }

        public static IEnumerable<HttpCall> FinishRecording()
        {
            return listener.Finish();
        }

        public static bool TryFinishRecording(out IEnumerable<HttpCall>? entries)
        {
            return listener.TryFinish(out entries);
        }
    }
}
#endif