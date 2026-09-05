# Inline Snapshots

C# and F# are supported.

Inline snapshots store the expected text inside the test file (.cs, .fs or .fsx) as a raw string literal, next to the code that produces it, instead of in a `.verified.` file on disk.


## Tooling status

 * [DiffEngineTray](https://github.com/VerifyTests/DiffEngine/blob/main/docs/tray.md): Use version 20.1.2 or higher.
 * [Verify.Terminal](https://github.com/VerifyTests/Verify.Terminal): Use version 0.9.1 or higher.
 * [Rider/R# Verify plugin](https://github.com/matkoch/jetbrains-plugin-verify): Support is in progress.


## Usage

Add `.Snapshot(...)` to any verification:

```cs
[Fact]
public Task MultiLine()
{
    var input = "line1\nline2";
    return Verify(input)
        .Snapshot();
}
```

Since the "expected" is omitted (or passed as `null` or `default`), the snapshot is treated as new; accepting it writes the literal into the source file:

snippet: InlineSample

Because `Snapshot` is a modifier rather than a separate entry point, it composes with every overload: `VerifyXml(...).Snapshot(...)`, `VerifyJson(...).Snapshot(...)`, `VerifyFile(...).Snapshot(...)`, and so on.

[Combinations](combinations.md) are included, since `Combination().Verify(...)` also returns a `SettingsTask`:

snippet: InlineCombinationSample

The verification pipeline is unchanged: the target is serialized and scrubbed exactly as for file snapshots, then compared against the literal. Line endings in the literal are normalized to `\n` before comparison, whether the source file uses `\r\n`, `\n` or a lone `\r`, so the comparison is not affected by the line endings of the source file.

Multiple inline verifications in a single test method are supported.


## Enabling inline snapshots globally

Most codebases want inline snapshots everywhere rather than one call at a time. Turn them on in a module initializer:

snippet: StaticInline

Every `Verify*` then uses an inline snapshot, unless it is declined by one of the rules below, and accepting one appends the `.Snapshot(...)` call to the verify invocation.

To decide per verification, pass a delegate:

snippet: StaticInlineDelegate

`extension` is that of the target that would be inlined, which is the first one.

Opt a single test out with `.NotInline()`, on either the instance or the fluent settings:

```cs
await Verify(target)
    .NotInline();
```

`NotInline` wins over both the global switch and an explicit `.Snapshot(...)`, with one exception: a `.Snapshot(...)` call combined with `UseUniqueDirectory` throws whether or not `NotInline()` is alongside it.


## Limiting the size of an inline snapshot

A long literal drowns the test method it sits in, which is the opposite of what an inline snapshot is for. `maxLines` keeps those on files:

snippet: StaticInlineMaxLines

A result of more than 30 lines then uses a `.verified.` file, and everything shorter is inlined.

The limit and the delegate combine as an and: the delegate picks the candidate tests, and the limit applies to what those produce.

`maxLines` counts the lines of the snapshot content, not the lines the literal occupies in source. A raw string literal adds two delimiter lines plus indentation on top of that. A single trailing newline starts no line, so it is not counted. Nothing is measured for width, so a one line snapshot is inlined however long that line is.

By default the limit routes new snapshots only, and a `.Snapshot(...)` call that already exists is left alone. Removing one rewrites source, so that is opted into separately:

snippet: StaticInlineApplyMaxLinesToExisting

Every existing literal over the limit then migrates, not only the ones whose content changed. A passing test stays passing through the move, since the literal it held is what seeds the new verified file, as described below. Nothing is rewritten on a build server, where such a test keeps using its literal.


## Parameterised tests

An inline snapshot is one literal at one call site, so it cannot hold a different expected value for each case of a [parameterised test](parameterised.md). What decides compatibility is whether the parameters reach the verified name:

 * **Parameters in the verified name**: not inlineable. The [global switch](#enabling-inline-snapshots-globally) declines such a test, which keeps using `.verified.` files, so turning inline on across a codebase leaves data-driven tests alone. An explicit `.Snapshot(...)` throws instead, since it is a stated intent that cannot be honoured.
 * **No parameters in the verified name**: inlineable. Every case already shares the one snapshot, which is exactly what a literal can represent.

Constructor arguments are treated the same as method parameters: they form part of the verified name unless ignored.

Dropping the parameters from the verified name therefore opts a parameterised test back in:

snippet: InlineIgnoreParametersSample

The APIs that do this are `IgnoreParameters()`, `IgnoreParametersForVerified()` and `IgnoreConstructorParameters()` on the instance settings, and `VerifierSettings.IgnoreParameters()` and `VerifierSettings.IgnoreConstructorParameters()` globally. Ignoring only some of the parameters is not enough, since the remaining ones still vary the verified name per case. `UseTextForParameters` counts as a parameter here for the same reason, while `UseFileName` pins the verified name so no parameter ever reaches it.

[`NotInline()`](#enabling-inline-snapshots-globally) is the other way out: it keeps a test on files without any of this applying.


## Which target is inlined

The **first** target is the inline snapshot. Any others are written to `.verified.` files as usual, keeping the names they would have had, so turning inline on never renames a snapshot file. That leaves a deliberate gap where the first target's file would have been: a verification that produced `#00`, `#01` and `#02` keeps `#01` and `#02` on disk.

If the first target is not text there is nothing for a literal to hold, so the global switch declines the verification and it falls back to files. Turning inline on therefore never breaks a test that emits a document or an image. An explicit `.Snapshot(...)` states an intent that cannot be honoured, so it throws instead: remove the `.Snapshot(...)` call, or add `.NotInline()`. `Target.DontInline` opts a target out as well, but only where the global switch is what turned inline on.


## Extensions that should never inline

A converter that splits one input into several text targets has no sensible first target: inlining the first page of a document and writing the rest to files helps nobody. Such a converter sets `DontInline` on the target that would otherwise be inlined:

```cs
new Target("md", page1)
{
    DontInline = true
}
```

The whole verification then falls back to files.


## Calling Verify through a wrapper

A test project often reaches Verify through a wrapper of its own, to convert a type Verify cannot take or to apply settings shared by a group of tests:

```cs
public static SettingsTask VerifyDocx(
    Document document,
    [CallerFilePath] string sourceFile = "",
    [CallerLineNumber] int lineNumber = 0)
{
    var stream = new MemoryStream();
    document.Save(stream, SaveFormat.Docx);
    stream.Position = 0;
    return Verify(stream, "docx", sourceFile: sourceFile, lineNumber: lineNumber);
}
```

Accepting a new inline snapshot chains a `.Snapshot(...)` call onto the call written in the **test**, not onto the one inside the wrapper. Three things have to hold for that to work:

 * The wrapper returns a `SettingsTask`. An `async Task` wrapper cannot: by the time it returns there is nothing left to chain onto, and the accepted source would not compile.
 * It forwards `sourceFile`, which is what puts the snapshot directory next to the test rather than next to the wrapper.
 * It forwards `lineNumber` as well. Forwarding one and not the other pairs a file with a line from a different file, and the call site that pair names is not the one the test wrote.

Then add the wrapper name:

snippet: StaticInlineEntryPoint

None of those three can be seen from the test file an accept has to rewrite, and often not even from the same project. So the name is the assertion that all three hold, and the wrapper's author is the one making it.

Leaving a wrapper unnamed is safe. Before a new snapshot is treated as inline, Verify reads the source line the call site points at, and the call found there has to be one an accept can append to: one of Verify's own entry points, or a wrapper named this way. An unnamed wrapper is neither, so its verifications keep using `.verified.` files, which is the outcome to prefer over an accept that writes source that does not compile. `.NotInline()` inside the wrapper asks for that same outcome outright.

Only a new snapshot is checked this way. An existing `.Snapshot(...)` call is already in the file, and already compiling.


## How a verification is routed

Every rule above feeds one decision, made per verification once the targets have been serialized and scrubbed:

```mermaid
graph TD
start["Verification"]
notInline{"NotInline() ?"}
start-->notInline

literal{"Snapshot(...)<br/>already in the source ?"}
notInline-- No -->literal

existing{"applyMaxLinesToExisting,<br/>over maxLines, and<br/>not a build server ?"}
literal-- Yes -->existing

migrate["Strip the Snapshot call.<br/>The literal seeds<br/>the verified file"]
existing-- Yes -->migrate

globalSwitch{"VerifierSettings.Inline()<br/>in a module initializer ?"}
literal-- No -->globalSwitch

compatible{"C# or F# source, a recognised test,<br/>no parameters in the verified name,<br/>and no UseUniqueDirectory ?"}
globalSwitch-- Yes -->compatible

accepted{"Delegate accepts, and the first target<br/>is text and not DontInline ?"}
compatible-- Yes -->accepted

within{"Within maxLines ?"}
accepted-- Yes -->within

callSite{"A verify entry point, or a<br/>declared wrapper, at the call site ?"}
within-- Yes -->callSite

inline["Inline snapshot"]
callSite-- Yes -->inline
existing-- No -->inline

isText{"First target<br/>is text ?"}
inline-->isText

compare["Compare against the literal.<br/>Accept rewrites the source"]
isText-- Yes -->compare

throws["Throws:<br/>Inline only supports text."]
isText-- No -->throws

file["Verified file"]
notInline-- Yes -->file
migrate-->file
globalSwitch-- No -->file
compatible-- No -->file
accepted-- No -->file
within-- No -->file
callSite-- No -->file
```

The checks that route a verification to files are silent rather than errors, so turning the switch on across a codebase leaves the tests it cannot represent alone. An explicit `.Snapshot(...)` is stricter: it throws for parameterised tests and for `UseUniqueDirectory`, since those are a stated intent that cannot be honoured.


## Accepting a snapshot

On a mismatch (or a new snapshot), Verify records the call site (file, line, and the literal's source text via `CallerArgumentExpression`) and produces a patch. Accepting the patch splices a new raw string literal into the source file, preserving the file's encoding, BOM, and line endings. The literal is then located by searching the file for that source text rather than by trusting the line number, so accepting one snapshot cannot misplace the next in a file where it has shifted the lines.

Accept mechanisms:

 * **AutoVerify**: with [AutoVerify](autoverify.md) enabled, the source file is rewritten immediately during the test run.
 * **[DiffEngineViewer](https://github.com/VerifyTests/DiffEngine/blob/main/docs/viewer.md)**: opens showing the received text against the expected text, with Accept and Discard. It ships inside the DiffEngine package, so it needs no install, and it runs on Windows, macOS and Linux. Several snapshots failing in one run queue into a single window.
 * **[DiffEngineTray](https://github.com/VerifyTests/DiffEngine/blob/main/docs/tray.md)**: pending snapshots appear under "Pending Snapshots" and can be accepted, discarded, or opened in the viewer.
 * **[Verify.Terminal](https://github.com/VerifyTests/Verify.Terminal)**: `dotnet verify review` steps through the pending snapshots at the command line, and `dotnet verify accept` takes them all. Both places an inline snapshot can be waiting are read, the queue and the staging directory described below, so what it lists does not depend on whether a tray is running. Needs version 0.9.1 or higher.
 * **[Rider/R# Verify plugin](https://github.com/matkoch/jetbrains-plugin-verify)**: accepts file snapshots today, and inline support is in progress.

One queue holds the pending snapshots, and it belongs to whichever process bound the port first, normally the tray since it starts at login. Everything else reaches that same queue over the socket instead of starting one of its own, so the tray, the viewer and Verify.Terminal are always working from one list rather than several that can disagree.

Nothing is written to disk for a pending inline snapshot: the patch is handed to the queue owner. Only when nothing owns a queue does Verify fall back to staging the received text, the expected text and the patch itself under `obj/VerifyInline/`, and launching whatever diff tool is configured.

On a build server, no source rewriting, review or staging occurs; the failure exception carries the full content.


## F#

F# test files (`.fs`, `.fsx`) work the same way, with one difference worth knowing because it decides what a literal means.

C# has raw strings: the compiler drops the line break after the opening delimiter and the indentation the closing delimiter sits at, and hands over the snapshot. F# has no such form. A triple-quoted string is verbatim, so what F# hands over still carries that line break and the indentation of every line. Writing the snapshot at the left margin instead is not an option either, since F#'s offside rule then rejects anything ending in a newline.

So the layout is stripped by agreement rather than by the compiler. Verify writes the shape C# would, and reads it back the same way. Accepting a two line snapshot into

```fs
let MyTest () =
    Verifier.Verify(value).Snapshot("old").ToTask()
```

produces

```fs
let MyTest () =
    Verifier.Verify(value).Snapshot(
        """
        line one
        line two
        """).ToTask()
```

That means a literal like that compares as the snapshot it looks like, rather than as the indented text F# produced.

Two consequences. Content ending in a newline is written as a blank line before the closing delimiter, exactly as in C#. And an F# `expected` argument is only the snapshot once Verify has read it: look at it any other way, in a debugger or by passing it somewhere else, and it still has its indentation.

Two further differences need nothing from the reader. F# does not implement `CallerArgumentExpression` (it warns FS0202), so a patch is anchored by the previous snapshot's value and by `CallerMemberName` rather than by the literal's source text. And `Snapshot` returns the `SettingsTask`, so an accepted snapshot is written in front of the `ToTask()` an F# test ends its chain with.


## Moving between file and inline snapshots

Both directions are handled without any manual file editing.

**File to inline.** The existing `.verified.` file for the inlined target is detected as stale and flows through the standard [Delete handling](exception-message-format.md): deleted automatically under AutoVerify, otherwise listed in the `Delete:` section and pended for review. A pending delete goes to the tray when one is running, and to the viewer otherwise, launching one if none is up, so it is reviewable with no tray installed. Files belonging to the other targets keep their names and are left alone.

This direction has no opt-in of its own, so a snapshot that shrinks back under a [`maxLines`](#limiting-the-size-of-an-inline-snapshot) limit returns to inline as soon as it does, leaving its file behind as a stale delete. A snapshot whose size hovers around the limit therefore moves each time it crosses.

**Inline to file.** When a `.Snapshot(...)` call exists but inline is off for that verification, the call is removed from the source and the snapshot runs as a normal file snapshot. The literal was the approved snapshot, so it seeds the verified file: an unchanged snapshot migrates without failing, and a changed one is an ordinary mismatch with the old and new text, accepted the usual way. Accepting a migration means committing both the source edit and the new `.verified.` file.

The two triggers for this direction are `.NotInline()` and an existing literal over a [`maxLines`](#limiting-the-size-of-an-inline-snapshot) limit.


## Exception message

Inline failures use the `InlineNew:` and `InlineNotEqual:` sections of the [exception message format](exception-message-format.md), and can be parsed with the Verify.ExceptionParsing package. Because only the first target is inlined, one message can carry both an inline section and the file sections for the remaining targets.
