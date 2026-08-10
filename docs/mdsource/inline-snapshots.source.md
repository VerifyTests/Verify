# Inline Snapshots

**Currently in 32.0.0-beta.1**

Inline snapshots store the expected text inside the C# test file as a raw string literal, next to the code that produces it, instead of in a `.verified.` file on disk.

Only C# source files and text results are supported.


## Usage

Add `.Snapshot(...)` to any verification:

snippet: InlineSample

Omitting the expected argument (or passing `null`) marks the snapshot as new; accepting it writes the literal into the source file.

Because `Snapshot` is a modifier rather than a separate entry point, it composes with every overload: `VerifyXml(...).Snapshot(...)`, `VerifyJson(...).Snapshot(...)`, `VerifyFile(...).Snapshot(...)`, and so on.

[Combinations](combinations.md) are included, since `Combination().Verify(...)` also returns a `SettingsTask`:

snippet: InlineCombinationSample

The verification pipeline is unchanged: the target is serialized and scrubbed exactly as for file snapshots, then compared against the literal. Line endings in the literal are normalized (`\r\n` to `\n`) before comparison, so the comparison is not affected by the line endings of the source file.

Multiple inline verifications in a single test method are supported.


## Enabling inline snapshots globally

Most codebases want inline snapshots everywhere rather than one call at a time. Turn them on in a module initializer:

snippet: StaticInline

Every `Verify*` then uses an inline snapshot, and accepting one appends the `.Snapshot(...)` call to the verify invocation.

To decide per verification, pass a delegate:

snippet: StaticInlineDelegate

`extension` is that of the target that would be inlined, which is the first one.

Opt a single test out with `.NotInline()`, on either the instance or the fluent settings:

```cs
await Verify(target)
    .NotInline();
```

`NotInline` wins over both the global switch and an explicit `.Snapshot(...)`.


## Parameterised tests

An inline snapshot is one literal at one call site, so it cannot hold a different expected value for each case of a [parameterised test](parameterised.md). What decides compatibility is whether the parameters reach the verified name:

 * **Parameters in the verified name**: not inlineable. The [global switch](#enabling-inline-snapshots-globally) declines such a test, which keeps using `.verified.` files, so turning inline on across a codebase leaves data driven tests alone. An explicit `.Snapshot(...)` throws instead, since it is a stated intent that cannot be honoured.
 * **No parameters in the verified name**: inlineable. Every case already shares the one snapshot, which is exactly what a literal can represent.

Constructor arguments are treated the same as method parameters: they form part of the verified name unless ignored.

Dropping the parameters from the verified name therefore opts a parameterised test back in:

snippet: InlineIgnoreParametersSample

The APIs that do this are `IgnoreParameters()`, `IgnoreParametersForVerified()` and `IgnoreConstructorParameters()` on the instance settings, and `VerifierSettings.IgnoreParameters()` and `VerifierSettings.IgnoreConstructorParameters()` globally. Ignoring only some of the parameters is not enough, since the remaining ones still vary the verified name per case. `UseTextForParameters` counts as a parameter here for the same reason, while `UseFileName` pins the verified name so no parameter ever reaches it.

`NotInline()` still wins over everything: it keeps a test on files without any of this applying.


## Which target is inlined

The **first** target is the inline snapshot. Any others are written to `.verified.` files as usual, keeping the names they would have had, so turning inline on never renames a snapshot file. That leaves a deliberate gap where the first target's file would have been: a verification that produced `#00`, `#01` and `#02` keeps `#01` and `#02` on disk.

If the first target is not text, the verification throws. Use `.NotInline()` for that test, or `Target.DontInline` for that extension.


## Extensions that should never inline

A converter that splits one input into several text targets has no sensible first target: inlining the first page of a document and writing the rest to files helps nobody. Such a converter sets `DontInline` on the target that would otherwise be inlined:

```cs
new Target("md", page1)
{
    DontInline = true
}
```

The whole verification then falls back to files.


## Accepting a snapshot

On a mismatch (or a new snapshot), Verify records the call site (file, line, and the literal's source text via `CallerArgumentExpression`) and produces a patch. Accepting the patch splices a new raw string literal into the source file, preserving the file's encoding, BOM, and line endings. The literal's location is found by content search, so line shifts from earlier edits do not break later ones.

Accept mechanisms:

 * **AutoVerify**: with [AutoVerify](autoverify.md) enabled, the source file is rewritten immediately during the test run.
 * **[DiffEngineViewer](https://github.com/VerifyTests/DiffEngine/blob/main/docs/viewer.md)**: opens showing the received text against the expected text, with Accept and Discard. It ships inside the DiffEngine package, so it needs no install, and it runs on Windows, macOS and Linux. Several snapshots failing in one run queue into a single window.
 * **[DiffEngineTray](https://github.com/VerifyTests/DiffEngine/blob/main/docs/tray.md)**: pending snapshots appear under "Pending Snapshots" and can be accepted, discarded, or opened in the viewer. The viewer owns the queue and the tray drives it over the same socket, so the two always agree.

Nothing is written to disk for a pending inline snapshot: the patch is handed to the viewer directly. Only when no viewer can be resolved does Verify fall back to staging the received and expected text under `obj/VerifyInline/` and launching whatever diff tool is configured.

On a build server, no source rewriting, review or staging occurs; the failure exception carries the full content.


## Moving between file and inline snapshots

Both directions are handled without any manual file editing.

**File to inline.** The existing `.verified.` file for the inlined target is detected as stale and flows through the standard [Delete handling](exception-message-format.md): deleted automatically under AutoVerify, otherwise listed in the `Delete:` section and pended in DiffEngineTray. Deletes still go through the tray; only the inline snapshot queue moved to the viewer. Files belonging to the other targets keep their names and are left alone.

**Inline to file.** When a `.Snapshot(...)` call exists but inline is off for that verification, the call is removed from the source and the snapshot runs as a normal file snapshot, which is then accepted the usual way. Nothing is rewritten on a build server.


## Exception message

Inline failures use the `InlineNew:` and `InlineNotEqual:` sections of the [exception message format](exception-message-format.md), and can be parsed with the Verify.ExceptionParsing package. Because only the first target is inlined, one message can carry both an inline section and the file sections for the remaining targets.
