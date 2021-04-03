# Logging Recording

Logging Recording allows, when a method is being tested, for any [logging](https://docs.microsoft.com/en-us/dotnet/core/extensions/logging) made as part of that method call to be recorded and verified.


## Usage

Call `LoggerRecording.Start();` to get an instance of the `LoggerProvider`. `LoggerProvider` implements both `ILogger` and `ILoggerProvider`.

The pass in the `LoggerProvider` instance to a class/method that write log entries:

snippet: LoggerRecording

Results in:

snippet: Tests.Logging.verified.txt


### Typed

A common pattern is to use a type logger (`Logger<T>`). `LoggerProvider` provides a builder method `CreateLogger<T>` to construct a `Logger<T>`:

snippet: LoggerRecordingTyped

Results in:

snippet: Tests.LoggingTyped.verified.txt