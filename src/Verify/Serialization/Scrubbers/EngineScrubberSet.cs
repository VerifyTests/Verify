// The merged, ordered view of all registered span scrubbers that apply to a single scrub operation.
// Levels merge in priority order: instance, extension-mapped instance, extension-mapped global, global.
// Inline scrubbers are ordered: unknown max length first, then descending max length, ties broken by
// level then registration order (stable via the collection index).
sealed class EngineScrubberSet
{
    public Scrubber[] LineDrops { get; }
    public Scrubber[] LineTransforms { get; }
    public Scrubber[] Inline { get; }

    // Values shorter than this cannot be changed by any scrubber in the set.
    // 0 disables the fast path. int.MaxValue when the set is empty.
    public int MinTrigger { get; }

    // Replicates RemoveEmptyLines trimming one leading and one trailing newline
    public bool TrimOuterEmptyLines { get; }

    public bool HasLinePhase => LineDrops.Length > 0 || LineTransforms.Length > 0;

    public bool IsEmpty =>
        LineDrops.Length == 0 &&
        LineTransforms.Length == 0 &&
        Inline.Length == 0;

    static EngineScrubberSet empty = new([], [], []);

    // A set with no scrubbers, for a pass that only needs the engine's pinned
    // trailing work (path replacements and newline normalization)
    public static EngineScrubberSet Empty => empty;

    EngineScrubberSet(Scrubber[] lineDrops, Scrubber[] lineTransforms, Scrubber[] inline)
    {
        LineDrops = lineDrops;
        LineTransforms = lineTransforms;
        Inline = inline;

        var minTrigger = int.MaxValue;
        foreach (var scrubber in lineDrops)
        {
            if (scrubber.Kind == ScrubberKind.LineDropEmpty)
            {
                TrimOuterEmptyLines = true;
            }

            minTrigger = Math.Min(minTrigger, MinLengthFor(scrubber));
        }

        foreach (var scrubber in lineTransforms)
        {
            minTrigger = Math.Min(minTrigger, MinLengthFor(scrubber));
        }

        foreach (var scrubber in inline)
        {
            minTrigger = Math.Min(minTrigger, MinLengthFor(scrubber));
        }

        MinTrigger = minTrigger;
    }

    static int MinLengthFor(Scrubber scrubber) =>
        scrubber.Kind switch
        {
            // A whitespace-only line needs at least one char to be droppable.
            // Removing empty lines from an empty string is the identity.
            ScrubberKind.LineDropEmpty => 1,
            // Predicates and transforms can act on empty lines
            ScrubberKind.LineDropSpan or
                ScrubberKind.LineDropString or
                ScrubberKind.LineTransformSpan or
                ScrubberKind.LineTransformString => 0,
            // Replace, Window, LineDropNeedles are at least 1. Match may be 0 (unknown)
            _ => scrubber.MinLength
        };

    static ConcurrentDictionary<string, EngineScrubberSet> globalExtensionCache = new(StringComparer.Ordinal);
    static EngineScrubberSet? globalOnlyCache;

    public static void InvalidateGlobalCache()
    {
        globalExtensionCache.Clear();
        globalOnlyCache = null;
    }

    // Whole-file pass: all four levels
    public static EngineScrubberSet ForExtension(VerifySettings settings, string extension)
    {
        var instance = settings.InstanceSpanScrubbers;
        List<Scrubber>? extensionInstance = null;
        settings.ExtensionMappedInstanceSpanScrubbers?.TryGetValue(extension, out extensionInstance);

        if (IsNullOrEmpty(instance) &&
            IsNullOrEmpty(extensionInstance))
        {
            return globalExtensionCache.GetOrAdd(
                extension,
                static ext =>
                {
                    VerifierSettings.ExtensionMappedGlobalSpanScrubbers.TryGetValue(ext, out var extensionGlobal);
                    return Build(null, null, extensionGlobal, VerifierSettings.GlobalSpanScrubbers);
                });
        }

        VerifierSettings.ExtensionMappedGlobalSpanScrubbers.TryGetValue(extension, out var extensionGlobalScrubbers);
        return Build(instance, extensionInstance, extensionGlobalScrubbers, VerifierSettings.GlobalSpanScrubbers);
    }

    // Property-value pass: instance and global only. Extension-mapped scrubbers
    // are excluded, matching the legacy pipeline. The merged set is cached on the
    // settings instance since this runs once per serialized string value.
    public static EngineScrubberSet ForPropertyValue(VerifySettings settings)
    {
        var instance = settings.InstanceSpanScrubbers;
        if (IsNullOrEmpty(instance))
        {
            return globalOnlyCache ??= Build(null, null, null, VerifierSettings.GlobalSpanScrubbers);
        }

        return settings.PropertyValueSetCache ??= Build(instance, null, null, VerifierSettings.GlobalSpanScrubbers);
    }

    static bool IsNullOrEmpty(List<Scrubber>? list) =>
        list == null ||
        list.Count == 0;

    // Test hook: a set over explicit scrubbers, bypassing the registration levels
    internal static EngineScrubberSet ForScrubbers(List<Scrubber> scrubbers) =>
        Build(scrubbers, null, null, []);

    static EngineScrubberSet Build(
        List<Scrubber>? instance,
        List<Scrubber>? extensionInstance,
        List<Scrubber>? extensionGlobal,
        List<Scrubber> global)
    {
        var count = (instance?.Count ?? 0) +
                    (extensionInstance?.Count ?? 0) +
                    (extensionGlobal?.Count ?? 0) +
                    global.Count;
        if (count == 0)
        {
            return empty;
        }

        var lineDrops = new List<Scrubber>();
        var lineTransforms = new List<Scrubber>();
        var inline = new List<(Scrubber Scrubber, int Order)>();
        var order = 0;

        void Collect(List<Scrubber>? scrubbers)
        {
            if (scrubbers == null)
            {
                return;
            }

            foreach (var scrubber in scrubbers)
            {
                if (scrubber.IsLineDrop)
                {
                    lineDrops.Add(scrubber);
                }
                else if (scrubber.IsLineTransform)
                {
                    lineTransforms.Add(scrubber);
                }
                else
                {
                    inline.Add((scrubber, order));
                }

                order++;
            }
        }

        Collect(instance);
        Collect(extensionInstance);
        Collect(extensionGlobal);
        Collect(global);

        inline.Sort((left, right) =>
        {
            var leftMax = left.Scrubber.MaxLength;
            var rightMax = right.Scrubber.MaxLength;

            // Unknown max length runs first
            if (leftMax.HasValue != rightMax.HasValue)
            {
                return leftMax.HasValue ? 1 : -1;
            }

            if (leftMax.HasValue)
            {
                // Longest max first
                var compare = rightMax!.Value.CompareTo(leftMax.Value);
                if (compare != 0)
                {
                    return compare;
                }
            }

            // Level then registration order
            return left.Order.CompareTo(right.Order);
        });

        var inlineArray = new Scrubber[inline.Count];
        for (var index = 0; index < inline.Count; index++)
        {
            inlineArray[index] = inline[index].Scrubber;
        }

        return new(
            [.. lineDrops],
            [.. lineTransforms],
            inlineArray);
    }
}
