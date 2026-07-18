# Verify 32.0.0 release notes

## Summary

This release replaces the internal text-scrubbing pipeline with a new **span based scrub engine** and
adds a public **`Scrubber`** API. The engine materializes each document once, tracks it as chunks, and
quarantines replacements so scrubbers no longer re-process each other's output. The result is large CPU
and allocation reductions for the built-in scrubbers, plus a first-class way to define custom scrubbers.

It is a **major version bump** because, although the public API is source compatible (see below), the
change is **behaviorally breaking in some configurations**: scrubber execution order and interaction are
now engine determined, so a subset of setups will produce different `.verified.` output and require
re-accepting snapshots.

The full behavior is documented in [scrubbers.md](/docs/scrubbers.md#the-scrub-engine).

## Why

The old pipeline was `StringBuilder` based. Every registered scrubber re-round-tripped the whole
document (`ToString()` + rebuild, or chunk-walking with cross-chunk carryover buffers), so cost scaled
with `scrubbers x document size` and allocated heavily even when nothing matched. The new engine does a
single pass with vectorized candidate scanning and returns the original string unchanged (zero copy)
when no scrubber fired.

## Performance

Measured on a 31 KB document (p99 of a scan of 33,707 real `.verified.` files), AMD Ryzen 9 5900X,
.NET 10, `MemoryDiagnoser`. "Old" runs the pre-engine implementations over identical input.

| Scenario | Old | New | Speedup | Allocation |
|---|--:|--:|--:|--:|
| Inline guid scan, no match | 105.5 us / 61.8 KB | 6.6 us / 1.2 KB | 16x | 51x less |
| Inline DateTime (ISO) scan, no match | 709.9 us / 122.4 KB | 72.7 us / 1.2 KB | 9.8x | 100x less |
| Inline DateTimeOffset, typical | 69.7 us / 15.5 KB | 4.1 us / 10.8 KB | 17x | — |
| `ScrubLinesContaining`, 508 lines | 16.2 us / 186 KB | 10.3 us / 59 KB | 1.6x | 3.2x less |
| Whole pipeline (guids + dates + line drops, one pass) | 884.0 us / 359 KB | 145.1 us / 103 KB | 6.1x | 3.5x less |
| Serialization string probe (per value, all misses) | 144 ns | 42 ns | 3.4x | — |

The one scenario that regresses is predicate line removal (`ScrubLines(Func<string, bool>)`) on small and
medium documents (~0.5 us at p50, ~10 us at 1,000 lines) — the cost of the composable pipeline. It wins
at scale (5x at 10,000 lines) and is avoidable via the new span predicate overload or
`ScrubLinesContaining`.

## Breaking changes

### API (source compatible, with obsoletions)

No public API was removed and no signatures changed incompatibly. However, the `ScrubberLocation`
parameter is now meaningless for the built-in scrub methods (ordering is engine determined), so those
overloads are marked `[Obsolete]`:

`ScrubLinesContaining`, `ScrubLines`, `ScrubLinesWithReplace`, `ScrubEmptyLines`, `ScrubInlineGuids`,
`ScrubInlineDates`, `ScrubInlineDateTimes`, `ScrubInlineDateTimeOffsets`, `ScrubMachineName`,
`ScrubUserName` (at global, instance, extension mapped, and fluent levels).

Existing calls still compile, but a call that passes a `ScrubberLocation` produces an obsolete warning.
Projects using `TreatWarningsAsErrors` will need to drop the `ScrubberLocation` argument. The
`ScrubberLocation` on the low-level `AddScrubber(Action<StringBuilder>, ...)` overloads is **unchanged**
and still honored.

### Behavior (snapshot output)

Even source-identical code can now produce different verified output in these cases. Where it applies,
re-accept the affected `.verified.` files.

1. **Order among built-in scrubbers is engine determined.** `ScrubberLocation.First` / `.Last` on a
   built-in scrub method is ignored. Registration level (global vs instance) no longer grants broad
   priority; inline scrubbers order by match length.
2. **Built-in scrubbers run before legacy `AddScrubber(Action<StringBuilder>)` scrubbers**, regardless
   of registration order. Custom `AddScrubber` delegates run after the engine and still see (and can
   modify) its output.
3. **Multiple `ScrubLinesWithReplace` now compose in registration order (FIFO).** Previously the default
   `ScrubberLocation.First` composed them in reverse.
4. **Quarantine.** Text produced by one built-in/`Scrubber` replacement is no longer re-examined by
   another. Legacy `AddScrubber` delegates are unaffected and can still re-scrub.
5. **Word boundary rule unified to `!char.IsLetterOrDigit`.** Inline guid scrubbing previously used
   `char.IsLetter || char.IsNumber`; a guid adjacent to an exotic numeric character (for example a
   superscript digit) is now scrubbed where it previously was not.
6. **Text is newline-normalized (`\r\n` and lone `\r` become `\n`) before scrubbers run.** A legacy
   `AddScrubber` delegate that matches a literal `"\r\n"` will no longer match.
7. **Directory/path replacement greedy trailing-separator absorption does not cross a quarantined
   replacement boundary.** Only relevant when another scrubber's replacement sits immediately after a
   scrubbed path.

Scrubbers that cannot be expressed on the engine (positional buffer edits, full-document reformatters,
multi-line regex) should stay on the `AddScrubber(Action<StringBuilder>)` overloads, which are unchanged.

## Migration

1. Update to `Verify 32.0.0`.
2. If the build fails with obsolete warnings, remove the `ScrubberLocation` argument from built-in scrub
   calls, for example `ScrubMachineName(ScrubberLocation.Last)` becomes `ScrubMachineName()`.
3. Run the test suite and re-accept any `.verified.` files that changed. Expected only in the
   configurations listed under "Behavior" above; a suite that does not rely on scrubber ordering or
   sugar-vs-legacy interaction should see no changes.
4. Optional: for hot custom line predicates, switch to the new span overloads to avoid a per-line string
   allocation, for example `ScrubLines((CharSpan line) => ...)`.

## New public API

Create a `Scrubber` via static factories and register it with `AddScrubber(Scrubber)` at any level
(global, instance, extension mapped, fluent):

```csharp
// Fixed-string replacement (optional comparison / word boundary)
settings.AddScrubber(Scrubber.Replace("find", "replacement"));

// Sliding window matcher (used internally by the guid and date scrubbers)
settings.AddScrubber(Scrubber.Window(minLength: 26, maxLength: 26, (window, counter, context) => ...));

// General search matcher (locates the next match within a segment)
settings.AddScrubber(Scrubber.Match((segment, counter, context, out index, out length, out replacement) => ...));

// Line scoped
settings.AddScrubber(Scrubber.RemoveLinesContaining("token"));
settings.AddScrubber(Scrubber.RemoveEmptyLines());
settings.AddScrubber(Scrubber.RemoveLines((CharSpan line) => ...));       // span predicate, no per-line alloc
settings.AddScrubber(Scrubber.ReplaceLines((CharSpan line) => ...));      // returns LineResult.Keep / Remove / Replace(text)
```

`ScrubLines` and `ScrubLinesWithReplace` also gained span-delegate overloads (`LineMatch` /
`LineReplace`); select them with an explicitly typed lambda parameter (`(CharSpan line) => ...`). Untyped
lambdas continue to bind the existing `string`-based overloads.

See [scrubbers.md](/docs/scrubbers.md) for full semantics.
