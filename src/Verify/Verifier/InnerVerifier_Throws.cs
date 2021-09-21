partial class InnerVerifier
{
    public Task Throws(Action target)
    {
        ScrubInnerVerifier();
        try
        {
            target();
        }
        catch (Exception exception)
        {
            return Verify(exception);
        }

        throw new("Did not throw.");
    }

    void ScrubInnerVerifier()
    {
        settings.ScrubLinesContaining("VerifyTests.InnerVerifier");
    }

    public Task Throws(Func<object?> target)
    {
        ScrubInnerVerifier();
        try
        {
            target();
        }
        catch (Exception exception)
        {
            return Verify(exception);
        }

        throw new("Did not throw.");
    }

    public async Task ThrowsValueTask(Func<ValueTask> target)
    {
        ScrubInnerVerifier();
        try
        {
            await target();
        }
        catch (Exception exception)
        {
            await Verify(exception);
            return;
        }

        throw new("Did not throw.");
    }

    public async Task ThrowsValueTask<T>(Func<ValueTask<T>> target)
    {
        ScrubInnerVerifier();
        try
        {
            await target();
        }
        catch (Exception exception)
        {
            await Verify(exception);
            return;
        }

        throw new("Did not throw.");
    }

    public async Task ThrowsTask(Func<Task> target)
    {
        ScrubInnerVerifier();
        try
        {
            await target();
        }
        catch (Exception exception)
        {
            await Verify(exception);
            return;
        }

        throw new("Did not throw.");
    }

    public async Task ThrowsTask<T>(Func<Task<T>> target)
    {
        ScrubInnerVerifier();
        try
        {
            await target();
        }
        catch (Exception exception)
        {
            await Verify(exception);
            return;
        }

        throw new("Did not throw.");
    }
}
