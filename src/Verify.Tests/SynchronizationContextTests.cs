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
