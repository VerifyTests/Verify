# Compared to ApprovalTests

Verify is heavily influenced by [ApprovalTests](https://github.com/approvals/ApprovalTests.Net). It is deigned to be an alternative to ApprovalTests.


## Differences to ApprovalTests


### No stack trace and symbols

ApprovalTests uses the stack trace and information from debug symbols. This results in a requirement for test assembly to have symbols enable and not be optimized.

Verify has no dependency on the stack trace and debug symbols.


### Async by default

The act of verification requires access to the file system and (possibly) the clipboard which both require blocking IO. As such the call to `Verify()` is async.


### Not attribute driven

ApprovalTests is, in the majority, configured via attributes.

Verify is configured using explicit code APIs and conventions.


### Clipboard on by default

When a test fails verification, the command to accept the new verified is copied to the clipboard.

In ApprovalTests this is an opt-in through an attribute.