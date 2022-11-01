// Non-nullable field is uninitialized.

#pragma warning disable CS8618

[UsesVerify]
public class ThrowsTests
{
    [Fact]
    public Task Throws() =>
        Verifier.Throws(MethodThatThrows);

    static void MethodThatThrows() =>
        throw new("The Message");

    [Fact]
    public Task ThrowsNested() =>
        Verifier.Throws(Nested.MethodThatThrows);

    static class Nested
    {
        public static void MethodThatThrows() =>
            throw new("The Message");
    }

    [Fact]
    public Task ThrowsArgumentException() =>
        Verifier.Throws(MethodThatThrowsArgumentException);

    static void MethodThatThrowsArgumentException() =>
        throw new ArgumentException("The Message", "The parameter");

    [Fact]
    public Task ThrowsInheritedArgumentException() =>
        Verifier.Throws(MethodThatThrowsArgumentNullException);

    static void MethodThatThrowsArgumentNullException() =>
        throw new ArgumentNullException("The parameter", "The Message");

    [Fact]
    public Task ThrowsAggregate() =>
        Verifier.Throws(MethodThatThrowsAggregate);

    static void MethodThatThrowsAggregate() =>
        throw new AggregateException(new Exception("The Message1"), new Exception("The Message2"));

    [Fact]
    public Task ThrowsEmptyAggregate() =>
        Verifier.Throws(MethodThatThrowsEmptyAggregate);

    static void MethodThatThrowsEmptyAggregate() =>
        throw new AggregateException();

    [Fact]
    public Task ThrowsTask() =>
        Verifier.ThrowsTask(TaskMethodThatThrows)
            .UniqueForRuntime()
            .ScrubLinesContaining("ThrowsAsync");

    static Task TaskMethodThatThrows() =>
        throw new("The Message");

    [Fact]
    public Task ThrowsWithInner() =>
        Verifier.Throws(MethodThatThrowsWithInner);

    static void MethodThatThrowsWithInner() =>
        throw new("The Message", new("Inner"));

    [Fact]
    public Task ThrowsTaskGeneric() =>
        Verifier.ThrowsTask(TaskMethodThatThrowsGeneric)
            .UniqueForRuntime()
            .ScrubLinesContaining("ThrowsAsync");

    static Task<string> TaskMethodThatThrowsGeneric() =>
        throw new("The Message");

    [Fact]
    public Task ThrowsValueTask() =>
        Verifier.ThrowsValueTask(ValueTaskMethodThatThrows)
            .UniqueForRuntime()
            .ScrubLinesContaining("ThrowsAsync");

    static ValueTask ValueTaskMethodThatThrows() =>
        throw new("The Message");

    [Fact]
    public Task ThrowsValueTaskGeneric() =>
        Verifier.ThrowsValueTask(ValueTaskMethodThatThrowsGeneric)
            .UniqueForRuntime()
            .ScrubLinesContaining("ThrowsAsync");

    static ValueTask<string> ValueTaskMethodThatThrowsGeneric() =>
        throw new("The Message");


    [Fact]
    public async Task ExceptionResult()
    {
        #region ExceptionResult

        var result = await Verifier.Throws(MethodThatThrows);
        Assert.NotNull(result.Exception);

        #endregion

        Assert.NotNull(result.Target);
    }
}