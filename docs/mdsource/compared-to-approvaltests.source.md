# Compared to ApprovalTests

Verify is heavily influenced by [ApprovalTests](https://github.com/approvals/ApprovalTests.Net). It is designed to be an alternative to ApprovalTests.


## Differences to ApprovalTests


### Multiple files

ApprovalTests supports producing a single file from a test.

Verify support multiple files from a test. For example a snapshot of a webpage can result in both the png and the html being output files.


### Extensibility

ApprovalTests extensibility model is primarily based on wrapping the top level api. This results in a limitation of the types of extensibility that can be achieved.

Verify is designed with extensibility in mind with many APIS to plug into. For example:

 * [Comparers](comparer.md).
 * [File naming](naming.md).
 * [Converters](converter.md).

This results in a simpler process for creating [Custom extensions for Verify](/#extensions)


### Object Serialization

Verify supports verification of any object through the use of Json.net


### No stack trace and symbols

ApprovalTests uses the stack trace and information from debug symbols. This results in a requirement for test assembly to have symbols enable and not be optimized.

Verify has no dependency on the stack trace and debug symbols.


### Async by default

The act of verification requires access to the file system and (possibly) the clipboard which both require blocking IO. As such the call to `Verify()` is async.


### Not attribute driven

ApprovalTests is, in the majority, configured via attributes.

Verify is configured using explicit code APIs and conventions.


### Clipboard and Diff tool on by default

When a test fails verification:

 * The command to accept the new verified is copied to the clipboard.
 * The difference between the received and verified files is displayed in a diff tool.

In ApprovalTests both these features are opt-in through attributes.