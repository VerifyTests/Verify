# Inline Snapshots

Inline snapshots store the expected text inside the C# test file as a raw string literal, next to the code that produces it, instead of in a `.verified.` file on disk.

Only C# source files and text results are supported.


## Usage

snippet: InlineSample

Omitting the expected argument (or passing `null`) marks the snapshot as new; accepting it writes the literal into the source file.

The verification pipeline is unchanged: the target is serialized and scrubbed exactly as for file snapshots, then compared against the literal. Line endings in the literal are normalized (`\r\n` to `\n`) before comparison, so the comparison is not affected by the line endings of the source file.

Multiple inline verifications in a single test method are supported.


## Accepting a snapshot

On a mismatch (or a new snapshot), Verify records the call site (file, line, and the literal's source text via `CallerArgumentExpression`) and produces a patch. Accepting the patch splices a new raw string literal into the source file, preserving the file's encoding, BOM, and line endings. The literal's location is found by content search, so line shifts from earlier edits do not break later ones.

Accept mechanisms:

 * **AutoVerify**: with [AutoVerify](autoverify.md) enabled, the source file is rewritten immediately during the test run.
 * **[DiffEngineViewer](https://github.com/VerifyTests/DiffEngine/blob/main/docs/viewer.md)**: opens showing the received text against the expected text, with Accept and Discard. It ships inside the DiffEngine package, so it needs no install, and it runs on Windows, macOS and Linux. Several snapshots failing in one run queue into a single window.
 * **[DiffEngineTray](https://github.com/VerifyTests/DiffEngine/blob/main/docs/tray.md)**: pending snapshots appear under "Pending Snapshots" and can be accepted, discarded, or opened in the viewer. The viewer owns the queue and the tray drives it over the same socket, so the two always agree.

Nothing is written to disk for a pending inline snapshot: the patch is handed to the viewer directly. Only when no viewer can be resolved does Verify fall back to staging the received and expected text under `obj/VerifyInline/` and launching whatever diff tool is configured.

On a build server, no source rewriting, review or staging occurs; the failure exception carries the full content.


## Multiple targets

When a verification produces multiple text targets (for example via `AppendContentAsFile`), all targets render into a single document separated by header lines:

```
---------- target#00.txt ----------
first content
---------- target#01.txt ----------
second content
```

Binary targets are not supported and produce an error.


## Moving a test from file snapshots to inline

When a test switches to inline snapshots, its existing `.verified.` files are detected as stale and flow through the standard [Delete handling](exception-message-format.md): deleted automatically under AutoVerify, otherwise listed in the `Delete:` section and pended in DiffEngineTray. Deletes still go through the tray; only the inline snapshot queue moved to the viewer.


## Exception message

Inline failures use the `InlineNew:` and `InlineNotEqual:` sections of the [exception message format](exception-message-format.md), and can be parsed with the Verify.ExceptionParsing package.
