# Scrubber API Migration Guide

The scrubber pipeline has been redesigned around three abstract base classes — `PatternScrubber`, `LineScrubber`, and `ContentScrubber` — replacing the previous `Action<StringBuilder, ...>` model. The new API gives the engine enough information to skip scrubbers whose match window can't fit in the input, run line-scoped scrubbers per line instead of over the whole buffer, and stop competing scrubbers once a range has been claimed.

This guide explains how to migrate custom scrubber code.


## Why the change

The old engine ran every registered scrubber in sequence over the full `StringBuilder`. There was no way to express that a scrubber's match could never exceed N chars, only matched within a single line, or could safely skip ranges already replaced by another scrubber. That meant unnecessary work — for example, the Guid scrubber paid the full chunked-scan cost on a 10-character input even though no match could ever fit.

The new engine knows:

 * The minimum and maximum length each pattern scrubber can match (skip the scrubber when input is shorter than `MinLength`).
 * Whether matches can span newlines (per-line fast path).
 * That once a position has been claimed by one pattern scrubber, no other pattern scrubber needs to look at it (no chaining).


## API summary

Three new abstract classes:

snippet: PatternScrubber

snippet: LineScrubber

snippet: ContentScrubber

Register via the same `AddScrubber` methods on `VerifySettings` and `VerifierSettings`, now overloaded to accept any of the three types.


## Ordering

`ScrubberLocation` is removed. The new ordering rules:

 * **Pattern scrubbers** are sorted by `MaxLength` descending (with `MinLength` descending as the tie-break). Longer / more specific matches win at overlapping positions. The order in which a project registered patterns no longer affects the outcome.
 * **Line scrubbers** run after pattern scrubbers, in registration order. Line scrubbers chain — each receives the line as transformed by the previous line scrubber.
 * **Content scrubbers** run before pattern and line scrubbers, in registration order.

Existing snapshots may shift on first run because of the order change. Re-run a project's tests after updating, review the `.received.*` files, and accept the new content where the diff is purely a result of the new ordering.


## Migrating custom scrubbers

### Pattern-style: find a fixed or bounded substring and replace it

Old:

```csharp
verifySettings.AddScrubber(builder =>
{
    var pattern = "TICKET-";
    var index = 0;
    while ((index = builder.IndexOf(pattern, index)) >= 0)
    {
        // ... locate the digits, replace
    }
});
```

New: subclass `PatternScrubber`. Declare the bounds, implement `TryMatch`, and let the engine drive the scan:

```csharp
sealed class TicketScrubber : PatternScrubber
{
    public override int MinLength => 8;       // "TICKET-1" minimum
    public override int MaxLength => 14;      // "TICKET-1234567" maximum
    public override bool SingleLine => true;

    public override bool TryMatch(
        ReadOnlySpan<char> source,
        int position,
        Counter counter,
        IReadOnlyDictionary<string, object> context,
        out int matchLength,
        out string? replacement)
    {
        if (!source.Slice(position).StartsWith("TICKET-"))
        {
            matchLength = 0;
            replacement = null;
            return false;
        }

        // ... determine matchLength and replacement
        matchLength = 14;
        replacement = "TICKET-{Scrubbed}";
        return true;
    }
}

verifySettings.AddScrubber(new TicketScrubber());
```

The engine will skip the scrubber when the remaining input is shorter than `MinLength`, and will only look at lines (instead of the whole buffer) when `SingleLine` is `true`.


### Line-style: drop or rewrite whole lines

Old:

```csharp
verifySettings.AddScrubber(builder => builder.FilterLines(line => line.StartsWith("INTERNAL:")));
```

New: subclass `LineScrubber`. Return `null` to drop, or a string to keep / replace:

```csharp
sealed class DropInternalLines : LineScrubber
{
    public override string? Process(
        ReadOnlySpan<char> line,
        Counter counter,
        IReadOnlyDictionary<string, object> context)
    {
        if (line.StartsWith("INTERNAL:"))
        {
            return null;
        }

        return line.ToString();
    }
}

verifySettings.AddScrubber(new DropInternalLines());
```


### Content-style: rewrite the whole buffer

Reach for `ContentScrubber` only when the transformation genuinely needs the entire content (for example, a stack trace cleanup). Pattern and line scrubbers should cover the common cases.

```csharp
sealed class RewriteHeader : ContentScrubber
{
    public override void Process(
        ReadOnlySpan<char> input,
        StringBuilder output,
        Counter counter,
        IReadOnlyDictionary<string, object> context)
    {
        // Read input, write transformed content to output.
        output.Append(input);
        output.Replace("X-Internal-Header:", "X-Header:");
    }
}

verifySettings.AddScrubber(new RewriteHeader());
```


## Obsolete `Action<StringBuilder, ...>` overloads

The legacy `AddScrubber(Action<StringBuilder>, ScrubberLocation)` overloads still compile but are marked `[Obsolete]`. They wrap the action as a `ContentScrubber` internally and continue to honor `ScrubberLocation` for ordering within the content-scrubber list. Migrate to one of the three abstract bases when next touching that code.


## Snapshot churn checklist

 * Re-run the test suite after updating.
 * For each `.received.*` file, confirm the diff is only re-ordering or non-overlapping replacement changes — not a missed scrubbing.
 * Accept `.received.*` over `.verified.*` only after a manual review.
 * If a scrubber that used to fire no longer does, check whether it relied on running before another scrubber's substitution. The new engine doesn't chain pattern scrubbers; fold the logic into a single scrubber, or use a `ContentScrubber` when whole-buffer ordering matters.
