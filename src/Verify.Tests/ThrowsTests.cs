// Non-nullable field is uninitialized.

#pragma warning disable CS8618

[UsesVerify]
public class ThrowsTests
{
    #region TestMethodThatThrows

    [Fact]
    public Task TestMethodThatThrows() =>
        Throws(MethodThatThrows);

    #endregion

    #region TestMethodThatThrowsIgnoreStackTraceFluent

    [Fact]
    public Task TestMethodThatThrowsIgnoreStackTraceFluent() =>
        Throws(MethodThatThrows)
            .IgnoreStackTrace();

    #endregion

    #region TestMethodThatThrowsIgnoreStackTraceSettings

    [Fact]
    public Task TestMethodThatThrowsIgnoreStackTraceSettings()
    {
        var settings = new VerifySettings();
        settings.IgnoreStackTrace();
        return Throws(MethodThatThrows, settings);
    }

    #endregion

    void IgnoreStackTraceSettingsGlobal()
    {
        #region IgnoreStackTraceGlobal

        VerifierSettings.IgnoreStackTrace();

        #endregion
    }

    #region MethodThatThrows

    static void MethodThatThrows() =>
        throw new("The Message");

    #endregion

#if NET8_0

    #region TestMethodThatThrowsTask

    [Fact]
    public Task TestMethodThatThrowsTask() =>
        ThrowsTask(MethodThatThrowsTask);

    #endregion

    #region MethodThatThrowsTask

    static async Task MethodThatThrowsTask()
    {
        await Task.Delay(1);
        throw new("The Message");
    }

    #endregion

    #region TestMethodThatThrowsValueTask

    [Fact]
    public Task TestMethodThatThrowsValueTask() =>
        ThrowsValueTask(MethodThatThrowsValueTask);

    #endregion

    #region MethodThatThrowsValueTask

    static async ValueTask MethodThatThrowsValueTask()
    {
        await Task.Delay(1);
        throw new("The Message");
    }

    #endregion

#endif
    [Fact]
    public Task ThrowsNested() =>
        Throws(Nested.MethodThatThrows);

    static class Nested
    {
        public static void MethodThatThrows() =>
            throw new("The Message");
    }

    [Fact]
    public Task ThrowsArgumentException() =>
        Throws(MethodThatThrowsArgumentException);

    static void MethodThatThrowsArgumentException() =>
        throw new ArgumentException("The Message", "The parameter");

    [Fact]
    public Task ThrowsInheritedArgumentException() =>
        Throws(MethodThatThrowsArgumentNullException);

    static void MethodThatThrowsArgumentNullException() =>
        throw new ArgumentNullException("The parameter", "The Message");

    [Fact]
    public Task ThrowsAggregate() =>
        Throws(MethodThatThrowsAggregate);

    static void MethodThatThrowsAggregate() =>
        throw new AggregateException(new Exception("The Message1"), new Exception("The Message2"));

    [Fact]
    public Task ThrowsEmptyAggregate() =>
        Throws(MethodThatThrowsEmptyAggregate);

    static void MethodThatThrowsEmptyAggregate() =>
        throw new AggregateException();

    [Fact]
    public Task TestThrowsTask() =>
        ThrowsTask(TaskMethodThatThrows)
            .UniqueForRuntime()
            .ScrubLinesContaining("ThrowsAsync");

    static Task TaskMethodThatThrows() =>
        throw new("The Message");

    [Fact]
    public Task ThrowsWithInner() =>
        Throws(MethodThatThrowsWithInner);

    static void MethodThatThrowsWithInner() =>
        throw new("The Message", new("Inner"));

    [Fact]
    public Task ThrowsTaskGeneric() =>
        ThrowsTask(TaskMethodThatThrowsGeneric)
            .UniqueForRuntime()
            .ScrubLinesContaining("ThrowsAsync");

    static Task<string> TaskMethodThatThrowsGeneric() =>
        throw new("The Message");

    [Fact]
    public Task TestThrowsValueTask() =>
        ThrowsValueTask(ValueTaskMethodThatThrows)
            .UniqueForRuntime()
            .ScrubLinesContaining("ThrowsAsync");

    static ValueTask ValueTaskMethodThatThrows() =>
        throw new("The Message");

    [Fact]
    public Task ThrowsValueTaskGeneric() =>
        ThrowsValueTask(ValueTaskMethodThatThrowsGeneric)
            .UniqueForRuntime()
            .ScrubLinesContaining("ThrowsAsync");

    static ValueTask<string> ValueTaskMethodThatThrowsGeneric() =>
        throw new("The Message");

    [Fact]
    public async Task ExceptionResult()
    {
        #region ExceptionResult

        var result = await Throws(MethodThatThrows);
        Assert.NotNull(result.Exception);

        #endregion

        Assert.NotNull(result.Target);
    }
}