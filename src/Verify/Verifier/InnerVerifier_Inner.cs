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
        Task VerifyInner(object? target, Func<Task>? cleanup, IEnumerable<Target> targets)
        {
            var targetList = targets.ToList();

            if (TryGetTargetBuilder(target, out var builder, out var extension))
            {
                ApplyScrubbers.Apply(extension, builder, settings);

                var received = builder.ToString();
                var stream = new Target(extension, received);
                targetList.Insert(0, stream);
            }

            return VerifyInner(targetList, cleanup);
        }

        async Task VerifyInner(List<Target> targetList, Func<Task>? cleanup)
        {
            VerifyEngine engine = new(settings, fileNameBuilder);

            targetList.AddRange(VerifierSettings.GetFileAppenders(settings));
            var builders = targetList
                .Select(
                    stream =>
                    {
                        return new ResultBuilder(
                            stream.Extension,
                            file => GetResult(settings, file, stream));
                    })
                .ToList();

            await engine.HandleResults(builders);

            if (cleanup != null)
            {
                await cleanup.Invoke();
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
                builder = new StringBuilder(stringTarget);
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

        static async Task<EqualityResult> GetResult(VerifySettings settings, FilePair filePair, Target conversionStream)
        {
            if (conversionStream.IsString)
            {
                var builder = new StringBuilder(conversionStream.StringData);
                ApplyScrubbers.Apply(conversionStream.Extension, builder, settings);
                return await Comparer.Text(filePair, builder.ToString(), settings);
            }

            var stream = conversionStream.StreamData;
#if NETSTANDARD2_0 || NETFRAMEWORK
            using (stream)
#else
            await using (stream)
#endif
            {
                stream.MoveToStart();
                return await Comparer.Streams(settings, stream, filePair);
            }
        }
    }
}