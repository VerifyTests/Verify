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

                return new("http", entries!);
            });
        }

        public static void StartRecording()
        {
            listener.Start();
        }

        public static IEnumerable<LogEntry> FinishRecording()
        {
            return listener.Finish();
        }

        public static bool TryFinishRecording(out IEnumerable<LogEntry>? entries)
        {
            return listener.TryFinish(out entries);
        }
    }
}