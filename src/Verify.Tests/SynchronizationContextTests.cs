public class SynchronizationContextTests
{
    // WinForms/WPF/Avalonia install a SynchronizationContext on the current thread when a control is
    // created. During a test that context has no running message pump, so if Verify's async IO
    // captured it, the continuation that writes the received file for a new or mismatched snapshot
    // would be posted to a pump that never runs - deadlocking the pipeline and the awaiting test.
    // SettingsTask.ToTask clears the ambient context so the pipeline runs free of it. This reproduces
    // that scenario without a UI framework: it records whether the pipeline ever posts a continuation
    // to the caller's context (dispatching it onward so the test itself never hangs, even on regression).
    [Fact]
    public async Task DoesNotCaptureCallerSynchronizationContext()
    {
        var context = new RecordingSynchronizationContext();
        var original = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(context);
        Task task;
        try
        {
            var settings = new VerifySettings();
            settings.DisableDiff();
            // A binary target with no baseline writes the received file via IoHelpers.WriteStream,
            // whose CopyToAsync onto an async FileStream suspends - the exact async IO that captured
            // the UI context and deadlocked. (Strings use a synchronous write path and would not.)
            task = Verify(new MemoryStream(new byte[1024 * 1024]), "bin", settings).ToTask();
        }
        finally
        {
            // Restore before awaiting so the test's own continuations are unaffected by the fake context.
            SynchronizationContext.SetSynchronizationContext(original);
        }

        await Assert.ThrowsAsync<VerifyException>(() => task);

        Assert.False(
            context.Posted,
            "Verify captured the caller's SynchronizationContext; its async IO must run free of it.");
    }

    // A context that isn't a UI framework's, such as the one TUnit's [STAThreadExecutor] runs a
    // test under, is the caller's again once ToTask returns, so the caller's await resumes on it:
    // on an STA thread, the thread that owns the test's windows. The pipeline still runs free of it.
    [Fact]
    public async Task CallerKeepsItsSynchronizationContext()
    {
        var context = new RecordingSynchronizationContext();
        var original = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(context);
        Task task;
        SynchronizationContext? afterToTask;
        try
        {
            task = VerifyMissingBinary();
            afterToTask = SynchronizationContext.Current;
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(original);
        }

        await Assert.ThrowsAsync<VerifyException>(() => task);

        Assert.Same(context, afterToTask);
        Assert.False(
            context.Posted,
            "Verify captured the caller's SynchronizationContext; its async IO must run free of it.");
    }

    // A UI framework's context stays cleared, since nothing pumps it during a test, and a caller's
    // continuation posted to it would never run.
    [Fact]
    public async Task UiSynchronizationContextStaysCleared()
    {
        var original = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(new Avalonia.Threading.AvaloniaSynchronizationContext());
        Task task;
        SynchronizationContext? afterToTask;
        try
        {
            task = VerifyMissingBinary();
            afterToTask = SynchronizationContext.Current;
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(original);
        }

        await Assert.ThrowsAsync<VerifyException>(() => task);

        Assert.Null(afterToTask);
    }

    // As in DoesNotCaptureCallerSynchronizationContext: no baseline, so the received file is written
    // through async IO that suspends.
    static Task<VerifyResult> VerifyMissingBinary()
    {
        var settings = new VerifySettings();
        settings.DisableDiff();
        return Verify(new MemoryStream(new byte[1024 * 1024]), "bin", settings).ToTask();
    }

    class RecordingSynchronizationContext : SynchronizationContext
    {
        public volatile bool Posted;

        public override void Post(SendOrPostCallback callback, object? state)
        {
            Posted = true;
            base.Post(callback, state);
        }
    }
}

// Stands in for Avalonia's context, which ToTask recognises by name.
namespace Avalonia.Threading
{
    class AvaloniaSynchronizationContext : SynchronizationContext;
}
