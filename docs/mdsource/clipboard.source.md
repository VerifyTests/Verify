# Clipboard

Verify makes use of the clipboard.

This is done via the [TextCopy project](https://github.com/CopyText/TextCopy).

Clipboard support is shipped via the [Verify.ClipboardAccept nuget](https://www.nuget.org/packages/Verify.ClipboardAccept/).

**An alternative to using the clipboard is the [DiffEngineTray tool](https://github.com/VerifyTests/DiffEngine/blob/master/docs/tray.md).**


## Enable

The clipboard behavior can be enabled using the following:

snippet: EnableClipboard


## Accept received

When a verification fails, a command to accept the received version is added to the clipboard:

On Windows:

> cmd /c move /Y "ReceivedFile" "VerifiedFile"

On Linux or OS:

> mv -f "ReceivedFile" "VerifiedFile"


## Cleanup dangling converter files

When the number of files outputted from a [converter](converter.md) reduces, a command to delete the extra files is added to the clipboard:

On Windows:

> cmd /c del "VerifiedFile"

On Linux or OS:

> rm -f "VerifiedFile"


## Custom Command

A custom command can be used by adding environment variables.


### Accept

Add a variable named `Verify.MoveCommand` where `{0}` and `{1}` will be replaced with the received and verified files respectively.


### Cleanup

Add a variable named `Verify.DeleteCommand` where `{0}` will be replaced with the file to be cleaned up.


### For a machine

Set a `Verify_DisableClipboard` environment variable to `true`. This overrides the above settings.