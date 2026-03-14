# Using Verify with AI Assistants

AI coding assistants (Claude Code, GitHub Copilot, Cursor, etc.) work well with Verify snapshot tests, but benefit from some project-level guidance. This page covers how to configure a project so AI assistants understand Verify's workflow.


## CLAUDE.md / AI context file

Add the following to the project `CLAUDE.md` (or equivalent AI context file). Adapt the test command and framework references to match the project.

~~~markdown
## Snapshot Testing (Verify)

This project uses [Verify](https://github.com/VerifyTests/Verify) for snapshot testing.

### Key concepts

- **`.verified.*` files** are the approved snapshots. They are committed to source control.
- **`.received.*` files** are the actual output from the latest test run. They are generated when a test fails (actual != expected) and are git-ignored.
- When a test fails, compare the `.received.*` file to the `.verified.*` file to understand the difference.

### Handling test failures

When a snapshot test fails:

1. Run the failing test and read the `.received.*` file that was generated.
2. Compare it to the corresponding `.verified.*` file.
3. Determine if the difference is expected (due to an intentional code change) or a bug.
   - **If expected**: copy the `.received.*` file over the `.verified.*` file to accept the new snapshot, or delete the `.verified.*` file and rename the `.received.*` file.
   - **If a bug**: fix the code, not the snapshot.
4. Never hand-edit `.verified.*` files to make tests pass. Always let Verify generate the correct output by running the test.

### Scrubbed values

Verify replaces non-deterministic values in snapshots with stable placeholders:

- GUIDs become `Guid_1`, `Guid_2`, etc.
- DateTimes become `DateTime_1`, `DateTime_2`, etc.
- File paths are replaced with tokens like `{SolutionDirectory}`, `{ProjectDirectory}`, `{TempPath}`.
- Custom scrubbers may replace other values.

These placeholders are intentional and ensure snapshots are deterministic across machines and runs.

### Exception message format

When a test fails, the exception message is machine-parsable with the following structure:

```
Directory: /path/to/test/project
New:
  - Received: TestClass.Method.received.txt
    Verified: TestClass.Method.verified.txt
NotEqual:
  - Received: TestClass.Method.received.txt
    Verified: TestClass.Method.verified.txt

FileContent:

New:

Received: TestClass.Method.received.txt
<file content here>

NotEqual:

Received: TestClass.Method.received.txt
<received content>
Verified: TestClass.Method.verified.txt
<verified content>
```

 * The `Directory:` line gives the base path for all relative file references.
 * `New` means no `.verified.` file exists yet (first run or new test).
 * `NotEqual` means the `.received.` and `.verified.` files differ.
 * `Delete` means a `.verified.` file is no longer produced by any test.
 * The `FileContent:` section contains the actual text content for quick comparison without needing to read files separately.

See [Exception Message Format](/docs/exception-message-format.md) for full details.

### Verified file conventions

- Encoding: UTF-8 with BOM
- Line endings: LF (not CRLF)
- No trailing newline

### Common patterns

```csharp
// Verify an object (serialized to JSON)
await Verify(myObject);

// Verify a string
await Verify("some text output");

// Verify with inline settings
await Verify(myObject)
    .ScrubLines(line => line.Contains("volatile"))
    .DontScrubDateTimes();

// Verify throws
await ThrowsTask(() => MethodThatThrows())
    .IgnoreStackTrace();
```
~~~


## GitHub Copilot / Copilot Chat context file

Add the following to the project COPILOT.md (or equivalent file Copilot can read). Adapt the test command and framework references to match the project.

~~~markdown
## Snapshot Testing (Verify) - Copilot guidance

This project uses [Verify](https://github.com/VerifyTests/Verify) for snapshot testing. Copilot and Copilot Chat use repository context files and README-like docs to guide code edits and chat actions.

### Recommended contents for COPILOT.md

- Suggested test command: `dotnet test src/Verify.slnx`
- Explain `.verified.*` files (approved snapshots) and `.received.*` files (generated when tests fail)
- Preferred workflow when tests fail:
  1. Run the tests and open any `*.received.*` files.
  2. Compare each `*.received.*` file to the matching `*.verified.*` file.
  3. If changes are intentional, accept by replacing the `.verified.*` file or run AutoVerify for bulk acceptance and review the git diff.
  4. If not intentional, fix the code rather than editing snapshots.
- Note: Copilot Chat may be able to run terminal commands in some IDEs; include explicit commands and file paths in prompts to allow manual execution when needed.

Suggested prompt template for Copilot Chat or the IDE assistant:

"Run `dotnet test src/Verify.slnx`, open any `*.received.*` files and show diffs vs the matching `*.verified.*` files. If differences are intentional, update the `*.verified.*` files (or run AutoVerify for bulk changes); otherwise fix the code."

~~~

### Tips for effective AI-assisted snapshot testing


### Let the test show what changed

When asked to update code that has Verify tests, run the tests first. The `.received.*` files show exactly what changed. This is faster and more reliable than trying to predict the snapshot output.


### Bulk snapshot updates

If a refactor changes many snapshots, use [AutoVerify](/docs/autoverify.md) temporarily to accept all changes in one pass, then review the diff in source control:

snippet: StaticAutoVerify


### Prompt strategies

When asking an AI assistant to work with Verify tests:

 * **"Run the tests and update the snapshots"** - Good for after intentional changes. The assistant should run the tests, review the `.received.*` files, and accept the expected changes.
 * **"The test is failing, fix it"** - The assistant should read both the `.received.*` and `.verified.*` files, understand the difference, then fix the code rather than the snapshot.
 * **"Add a new test for X"** - The assistant should write the test, run it (it will fail on first run since there is no `.verified.*` file yet), then accept the generated `.received.*` file as the new snapshot.


### GitHub Copilot

GitHub Copilot (including Copilot Chat) can follow the same workflow as other AI assistants. When asking Copilot to modify code or tests, have it:

- Run the project tests: `dotnet test src/Verify.slnx` (or your project's test command).
- If tests fail, open any generated `*.received.*` files and compare them to the matching `*.verified.*` files.
- Only accept/update `.verified.*` files when changes are intentional; otherwise fix the code.
- For large refactors, use AutoVerify and review the diff in source control.

Suggested prompt (for Copilot Chat or IDE assistant):

"Run `dotnet test src/Verify.slnx`, open any `*.received.*` files and show diffs vs the matching `*.verified.*` files. If differences are intentional, update the `*.verified.*` files (or run AutoVerify for bulk changes); otherwise fix the code."

Note Copilot may not have terminal access in some setups — include explicit commands and file paths in prompts so manual execution is possible.

### File location

Snapshot files live next to the test source file by default. For a test in `Tests/MyTests.cs`, the verified file will be at `Tests/MyTests.MethodName.verified.txt` (or `.json`, `.xml`, etc. depending on the content type).
