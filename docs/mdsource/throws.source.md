# Verify a method that throws an Exception

Given a method that throws an Exception

snippet: MethodThatThrows

That exception behavior can be verified using `Verify.Throws`:

snippet: TestMethodThatThrows

Resulting in the following snapshot file:

snippet: ThrowsTests.TestMethodThatThrows.verified.txt


## IgnoreStackTrace

Often the exception stack trace can be noisy and fragile to causing false failed tests. To exclude it use `IgnoreStackTrace`:


### Fluent API

snippet: TestMethodThatThrowsIgnoreStackTraceFluent


### Settings api

snippet: TestMethodThatThrowsIgnoreStackTraceSettings


### Globally

snippet: IgnoreStackTraceGlobal


### Result

snippet: ThrowsTests.TestMethodThatThrowsIgnoreStackTraceFluent.verified.txt


## Async

There are specific named `Throws*` method for methods that return a `Task` or a `ValueTask`.


### Task

snippet: MethodThatThrowsTask

snippet: TestMethodThatThrowsTask


### ValueTask

snippet: MethodThatThrowsValueTask

snippet: TestMethodThatThrowsValueTask