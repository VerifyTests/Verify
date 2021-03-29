using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerifyTests
{
    partial class InnerVerifier
    {
        async Task VerifyInner(object? target, Func<Task>? cleanup, IEnumerable<Target> targets)
        {
            var targetList = targets.ToList();

            if (TryGetTargetBuilder(target, out var builder, out var extension))
            {
                ApplyScrubbers.Apply(extension, builder, settings);

                var received = builder.ToString();
                Target stream = new(extension, received);
                targetList.Insert(0, stream);
            }

            targetList.AddRange(VerifierSettings.GetFileAppenders(settings));

            VerifyEngine engine = new(settings, fileNameBuilder);

            await engine.HandleResults(targetList);

            if (cleanup != null)
            {
                await cleanup();
            }

            await engine.ThrowIfRequired();
        }

        bool TryGetTargetBuilder(object? target, [NotNullWhen(true)] out StringBuilder? builder, [NotNullWhen(true)] out string? extension)
        {
            var appends = VerifierSettings.GetJsonAppenders(settings);

            var hasAppends = appends.Any();

            if (target == null)
            {
                if (!hasAppends)
                {
                    builder = null;
                    extension = null;
                    return false;
                }

                extension = "txt";
                if (VerifierSettings.StrictJson)
                {
                    extension = "json";
                }

                builder = JsonFormatter.AsJson(
                    null,
                    settings.serialization.currentSettings,
                    appends,
                    settings);
                return true;
            }

            if (!hasAppends && target is string stringTarget)
            {
                builder = new(stringTarget);
                builder.FixNewlines();
                extension = settings.ExtensionOrTxt();
                return true;
            }

            extension = "txt";

            if (VerifierSettings.StrictJson)
            {
                extension = "json";
            }

            builder = JsonFormatter.AsJson(
                target,
                settings.serialization.currentSettings,
                appends,
                settings);

            return true;
        }
    }
}