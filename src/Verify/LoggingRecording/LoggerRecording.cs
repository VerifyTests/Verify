using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace VerifyTests
{
    public static class LoggerRecording
    {
        static AsyncLocal<LoggerProvider?> local = new();

        public static LoggerProvider Start(LogLevel logLevel = LogLevel.Information)
        {
            return local.Value = new(logLevel);
        }

        public static bool TryFinishRecording(out IEnumerable<object>? entries)
        {
            var provider = local.Value;

            if (provider == null)
            {
                local.Value = null;
                entries = null;
                return false;
            }

            entries = provider.entries.ToArray();
            local.Value = null;
            return true;
        }
    }
}