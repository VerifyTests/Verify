# Diff Tool

When a test fails verification the difference between the received and verified files is displayed in a diff tool.


## Start Behavior


### Not Running

If no diff tool is running for the comparison of the current verification (per test), a new diff tool instance will be launched.


### Is Running

If a diff tool is running for the comparison of the current verification (per test), the following logic will be applied:

| Auto Refresh | Mdi   | Start Tool                               |
|--------------|-------|------------------------------------------|
| true         | true  | No action. Current instance will refresh |
| true         | false | No action. Current instance will refresh |
| false        | true  | Open new instance                        |
| false        | false | Kill current and open new instance       |

This allows, in most cases, for no manual closing of the diff tool to be required.

Note that the above "Is Running" detection behavior is currently supported on Windows. On Linux and OSX, a new instance is always started.


## Supported Diff tools:

include: diffTools


## Disable Diff

snippet: DisableDiff