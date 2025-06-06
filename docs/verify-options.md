<!--
GENERATED FILE - DO NOT EDIT
This file was generated by [MarkdownSnippets](https://github.com/SimonCropp/MarkdownSnippets).
Source File: /docs/mdsource/verify-options.source.md
To change this file edit the source file and then run MarkdownSnippets.
-->

# Verify Options


## OnHandlers

 * `OnVerify` takes two actions that are called before and after each verification.
 * `OnFirstVerify` is called when there is no verified file.
 * `OnVerifyMismatch` is called when a received file does not match the existing verified file.


### AutoVerify

OnHandlers are called before [AutoVerify](autoverify.md) logic being applied. So for example in the case of `OnVerifyMismatch`, both the received and verified file will exist at the point `OnVerifyMismatch` is called. Immediately after received will be used to overwrite verified.


### Globally

<!-- snippet: OnStaticHandlers -->
<a id='snippet-OnStaticHandlers'></a>
```cs
public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings.OnVerify(
            before: () => Debug.WriteLine("before"),
            after: () => Debug.WriteLine("after"));
        VerifierSettings.OnFirstVerify(
            (receivedFile, receivedText, autoVerify) =>
            {
                Debug.WriteLine(receivedFile);
                Debug.WriteLine(receivedText);
                return Task.CompletedTask;
            });
        VerifierSettings.OnVerifyMismatch(
            (filePair, message, autoVerify) =>
            {
                Debug.WriteLine(filePair.ReceivedPath);
                Debug.WriteLine(filePair.VerifiedPath);
                Debug.WriteLine(message);
                return Task.CompletedTask;
            });
    }
}
```
<sup><a href='/src/ModuleInitDocs/OnHandlers.cs#L4-L32' title='Snippet source file'>snippet source</a> | <a href='#snippet-OnStaticHandlers' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Instance

<!-- snippet: OnInstanceHandlers -->
<a id='snippet-OnInstanceHandlers'></a>
```cs
[Fact]
public Task OnCallbacks()
{
    var settings = new VerifySettings();
    settings.OnVerify(
        before: () => Debug.WriteLine("before"),
        after: () => Debug.WriteLine("after"));
    settings.OnFirstVerify(
        (receivedFile, receivedText, autoVerify) =>
        {
            Debug.WriteLine(receivedFile);
            Debug.WriteLine(receivedText);
            return Task.CompletedTask;
        });
    settings.OnVerifyMismatch(
        (filePair, message, autoVerify) =>
        {
            Debug.WriteLine(filePair.ReceivedPath);
            Debug.WriteLine(filePair.VerifiedPath);
            Debug.WriteLine(message);
            return Task.CompletedTask;
        });

    return Verify("value", settings);
}
```
<sup><a href='/src/Verify.Tests/Tests.cs#L138-L166' title='Snippet source file'>snippet source</a> | <a href='#snippet-OnInstanceHandlers' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Fluent

<!-- snippet: OnFluentHandlers -->
<a id='snippet-OnFluentHandlers'></a>
```cs
[Fact]
public Task OnFluentCallbacks() =>
    Verify("value")
        .OnVerify(
            before: () => Debug.WriteLine("before"),
            after: () => Debug.WriteLine("after"))
        .OnFirstVerify(
            (receivedFile, receivedText, autoVerify) =>
            {
                Debug.WriteLine(receivedFile);
                Debug.WriteLine(receivedText);
                return Task.CompletedTask;
            })
        .OnVerifyMismatch(
            (filePair, message, autoVerify) =>
            {
                Debug.WriteLine(filePair.ReceivedPath);
                Debug.WriteLine(filePair.VerifiedPath);
                Debug.WriteLine(message);
                return Task.CompletedTask;
            });
```
<sup><a href='/src/Verify.Tests/Tests.cs#L168-L192' title='Snippet source file'>snippet source</a> | <a href='#snippet-OnFluentHandlers' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## OmitContentFromException

By default, when a verify mismatch occurs for text, the content of the received and verified files is included in the exception that is thrown. This results in that text being included in test runners and build output. To omit the content use `VerifierSettings.OmitContentFromException`.


## DisableDiff

To disable diff launching:

<!-- snippet: DisableDiff -->
<a id='snippet-DisableDiff'></a>
```cs
var settings = new VerifySettings();
settings.DisableDiff();
```
<sup><a href='/src/Verify.Tests/Snippets/Snippets.cs#L120-L125' title='Snippet source file'>snippet source</a> | <a href='#snippet-DisableDiff' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->
