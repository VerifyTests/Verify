# Bug findings — 2026-08-16 review sweep

All items verified against source at HEAD `e11017db5`; several also runtime-reproduced.

## High impact

All six resolved 2026-08-16 (five fixed here; the inline item resolved as by-design, see below). Verify.Tests, StaticSettingsTests, Verify.NUnit.Tests, and DiffEngine InlinePatcherTests all pass.

- [x] **Inline Append anchoring breaks for same-file helpers** — resolved as **by design**, not a bug.
  A test that reaches `Verify` through a same-file helper that does not forward the caller-info attributes hits the member floor in DiffEngine's `InlinePatcher.FindCalls` and fails with an explicit NotFound. Decision: that layout is unsupported on purpose — the helper's single `Verify` call is shared by every test that uses it, so an inline snapshot spliced into it could only ever be right for one caller, and weakening the floor to reach it would let a genuinely stale hint patch a neighboring test. Helpers that want inline snapshots forward the caller info and carry a `Verify`/`Throws` prefix, like any custom entry point. Pinned by `AppendDoesNotReachAHelperDeclaredAboveTheMember` and `AppendPrefersACallInsideTheMemberOverAHelperAbove` in DiffEngine's `InlinePatcherTests`.

- [x] **Stray extension-less snapshot file crashes every run of its test.**
  `Verify/Naming/MatchingFileFinder.cs:53` — `SubStringEquals` slices `match.Length` chars with no length guard. Win32 pattern semantics make `EnumerateFiles(dir, "{prefix}*.verified.*")` also match a file named exactly `{prefix}.verified` (no trailing extension), which is one char shorter than the pattern → `ArgumentOutOfRangeException` from `FindVerified`/`DeleteReceived` before the test body runs. Runtime-reproduced independently by two reviewers. Fix: length-check before `Slice`.

- [x] **Static `UniqueForTargetFramework(Assembly)` / `UniqueForTargetFrameworkAndVersion(Assembly)` ignore the passed assembly.**
  The static APIs store the framework name on `SharedNamer` (`Verify/Naming/VerifierSettings.cs:286,297`), but the only consumer reads the instance field directly (`Verify/Naming/PrefixUnique.cs:44`) instead of `ResolveUniqueForTargetFrameworkName()` (`Verify/Naming/Namer.cs:73`), which has **zero call sites**. Result: the suffix falls back to the test assembly's TFM (or throws "no TargetFrameworkAttribute found" if the test assembly lacks one). Fix: use the resolver, mirroring `AppendAssemblyConfiguration`.

- [x] **File appenders bypass scrubbing on the post-conversion stream path.**
  `Verify/Verifier/InnerVerifier_Inner.cs:297-306` — `GetTargets` concatenates `GetFileAppenders(settings)` into the list, then returns everything untouched when `doExtensionConversion == false`. The early-return comment is true for the converted targets but the appenders have had neither scrubbing nor newline normalization. Scenario: `Verify(pngStream, "png")` (registered stream converter) + `settings.AppendContentAsFile("a\r\nb")` → received `.txt` contains literal `\r\n` and unscrubbed content; accepting yields `VerifiedLineEndingException` on the next run. Same appender without conversion is normalized correctly.

- [x] **Inline date scrubbing silently misses matches — three window-bounds bugs.**
  `ScrubEngine.TryFindWindowMatch` only probes window lengths `min..max`, so any wrong bound means timestamps are silently left unscrubbed:
  - `Verify/Serialization/Scrubbers/DateFormatLengthCalculator.cs:77-82` — single `t` renders only the first char of the AM/PM designator, but the calculator adds the full designator lengths. `"h:mm t"` → `9:30 A` (6 chars) vs computed min 7: single-digit hours never scrub while `12:30 P` does.
  - `Verify/Serialization/Scrubbers/DateFormatLengthCalculator.cs:349-350` — only nominative month names feed the bounds, but .NET renders *genitive* names (`MonthGenitiveNames`) for `MMMM` next to a day component. cs-CZ: `"15 listopadu 2020"` = 17 chars vs computed max 16 → November dates never scrub.
  - `Verify/Serialization/Scrubbers/DateFormatLengthCalculator.cs:198-204` — quoted literals count escape backslashes (`ParseQuoteString` length minus only the two quotes). `'o\'clock'` overcounts min by 1 per escape → the scrubber never matches at all.

- [x] **`IgnoreParameters()` "ignore all" sentinel destroyed by merging.**
  `Verify/Naming/FileNameBuilder.cs:68-97` — the documented empty-set = ignore-all sentinel (honored via `Count == 0` at line 136) is concatenated with global ignores and/or constructor-parameter names, producing a non-empty set. `IgnoreParameters()` + global `IgnoreParameters("a")` on `(a, b)` keeps `_b=...` in the verified name; `IgnoreParameters()` + `IgnoreConstructorParameters()` resurrects method parameters.

## Correctness, narrower triggers

- [x] **Negative UTC offsets formatted with current culture.**
  `Verify/Serialization/DateFormatter_DateTimeOffset.cs:78,81` — plain interpolation (`$"{offset.Hours:0}"`) uses `CurrentCulture` while every other call in the file passes `Culture.InvariantCulture`. Under `ar-SA` the negative sign renders as invisible U+061C + `-`, so a `DateTimeOffset` parameter with offset `-05:00` produces a filename that never matches a snapshot committed from an en-US machine. Also leaks into snapshot content via `Convert` when date scrubbing is off. Wider than first recorded: `sv-SE` renders U+2212 MINUS SIGN and `fa-IR` U+200E + U+2212, so the sign is visibly wrong, not just invisible.

- [x] **Sub-millisecond date parameters collide.**
  `Verify/Serialization/DateFormatter_DateTime.cs` (and the `DateTimeOffset` twin) use `Second == 0` / `Millisecond == 0` to omit the fraction, but sub-millisecond ticks leave those properties 0. `AddTicks(1)` and `AddTicks(2)` cases format identically → spurious "prefix has already been used" (or silent sharing of one verified file). Correct check is ticks-based.

- [x] **`#` in parameter values collides with the indexed-target namespace.**
  `Verify/Naming/MatchingFileFinder.cs:9,20` — `indexedPattern: "{prefix}#"` matches by prefix, and `#` is not sanitized. Cases `"x"` and `"x#1"` on one method: running `"x"` deletes `C.M_p=x#1.received.txt` and sweeps `C.M_p=x#1.verified.txt` into the stale set (deleted under AutoVerify).

- [x] **Trimmed fraction format collapses into a standard format specifier.**
  `Verify/Serialization/Scrubbers/DateMatchers.cs:269-297` (consumed at 118-127) — `ScrubInlineDateTimes("s.F")` builds a secondary scrubber for `"s"`; length-1 formats are standard specifiers, so it scrubs every full sortable date-time in the output. `"H.F"` trims to `"H"` and throws `Invalid format: H` at registration despite passing up-front validation.

- [ ] **`MemberConverter` has no exact-type precedence.**
  `Verify/Serialization/VerifierSettings_MemberConverter.cs:13-25` — first registered entry with `IsAssignableFrom` wins, so a base-interface converter registered earlier permanently shadows a more specific one. Contrast `TryGetScrubOrIgnoreByMemberOfType`, which checks the exact declaring type first.

- [ ] **Registering an `IgnoreInstance` predicate disables empty-collection ignoring for that type.**
  `Verify/Serialization/SerializationSettings_ShouldIgnore.cs:26-40` — when predicates exist but none match, the early `return false` skips the `ignoreEmptyCollections` check at 42-47. An empty `List<string>` starts appearing as `[]` merely because an unrelated predicate was registered.

- [x] **Combinations name cache collapses distinct keys.**
  `Verify/Combinations/CombinationResultsConverter.cs:32-54` — `Dictionary<object, string>` keyed on the boxed value: `DateTime.Equals` ignores `Kind`, `DateTimeOffset.Equals` compares only the instant, while the rendered names include Kind/offset. Inputs `2000-01-01 Utc` and `2000-01-01 Local` both get labeled `2000-01-01Utc`.

- [x] **MSTest overloaded test methods resolve to the wrong `MethodInfo`.**
  `Verify.MSTest/TestExecutionContext.cs:24-30` — `FindMethod` returns the first name match, ignoring parameters. With two `[DataRow]` overloads of one name, the parameter-count guard in `Verifier.BuildVerifier` mismatches for one of them → `SetParameters` silently skipped → both overloads collide on one snapshot prefix.

- [x] **`Delete:` section drops subdirectories, breaking the parse round-trip.**
  `Verify/Verifier/VerifyExceptionMessageBuilder.cs:62` emits `Path.GetFileName(file)` while the other sections emit directory-relative paths, and `Verify.ExceptionParsing/Parser.cs:109` reconstructs with `Path.Combine(directory, name)`. For `UseUniqueDirectory()`/`VerifyDirectory` tests, a stale `{Directory}\Type.Method\old.verified.txt` parses back as the nonexistent `{Directory}\old.verified.txt`; same-named files in different subdirectories collapse.

- [x] **`ThrowIfVerifyHasBeenRun` blames the caller instead of the API.**
  `Verify/Verifier/InnerVerifier.cs:35-38` — `new StackTrace(1, false)` already skips the guard, so frame 0 is the guarded API; `GetFrame(1)` fetches the API's caller. The message names the user's own method as "The API". Runtime-reproduced (Debug and Release). Fix: `GetFrame(0)`.

## Minor / edge cases

- [x] **Stack-trace scrubber destroys paren-less `at` frames** (NativeAOT: `at MyApp!<BaseAddress>+0x1a2b3c`).
  `Verify/Serialization/Scrubbers/ScrubStackTrace.cs:36-58` — `IndexOf('(')`/`')'` return −1, slice keeps zero chars → frame becomes an empty line, or the literal `...)` with `removeParams: true`.

- [x] **MSTest source generator ignores `record` test classes.**
  `Verify.MSTest.SourceGenerator/UsesVerifyGenerator.cs:125-126` — only `ClassDeclarationSyntax` is eligible; `[UsesVerify] [TestClass] partial record` compiles then fails at runtime with the misleading "TestContext is null" error. `Parser.GetParentClasses` similarly stops at a `record struct` parent.

- [x] **Unclosed JSON object for empty `CombinationResults`.**
  `Verify/Combinations/CombinationResultsConverter.cs:8-14` — `WriteStartObject()` then early `return` with no `WriteEndObject()`. Only reachable by constructing `CombinationResults([], ...)` directly.

- [x] **`FlattenMessage` omits the joining space after a line ending in `.`.**
  `Verify/Combinations/CombinationResultsConverter.cs:168-183` — two-line messages (net48 `ArgumentNullException`) render as `"Value cannot be null.Parameter name: p"`.

- [x] **Negative sub-hour offsets render unsigned.**
  `Verify/Serialization/DateFormatter_DateTimeOffset.cs:74-82` — `TimeSpan.FromMinutes(-30)` renders `0-30` (positive twin is `+0-30`). No real timezone in that range; constructed offsets only.

- [x] **Mismatch crash for handle-based `FileStream` received streams.**
  `Verify/Compare/FileComparer.cs:50` — NotEqual fast path copies by `fileStream.Name` with no fallback; handle-based streams have `Name == "[Unknown]"`. First (New) run succeeds via the guarded `IoHelpers.WriteStream` path; later mismatches throw the generic "Failed to compare files".

- [x] **`PrefixUnique` set is case-sensitive on case-insensitive filesystems.**
  `Verify/Naming/PrefixUnique.cs:3` — methods `Foo` and `foo` map to the same files on NTFS/APFS but pass the uniqueness check and silently clobber each other.

- [x] **`Counter` caches mix `Interlocked` counters with unsynchronized `Dictionary` writes.**
  `Verify/Counter_*.cs` → `Extensions.cs:155-164` — concurrent `Counter.Current.Next(...)` calls from parallel user code inside one test can corrupt the plain `Dictionary`.
