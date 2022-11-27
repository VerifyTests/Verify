namespace VerifyTests;

partial class InnerVerifier
{
    public Task<VerifyResult> Throws(Action target)
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

    void ScrubInnerVerifier() =>
        settings.ScrubLinesContaining("VerifyTests.InnerVerifier");

    public Task<VerifyResult> Throws(Func<object?> target)
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

    public async Task<VerifyResult> ThrowsValueTask(Func<ValueTask> target)
    {
        ScrubInnerVerifier();
        try
        {
            await target();
        }
        catch (Exception exception)
        {
            return await Verify(exception);
        }

        throw new("Did not throw.");
    }

    public async Task<VerifyResult> ThrowsValueTask<T>(Func<ValueTask<T>> target)
    {
        ScrubInnerVerifier();
        try
        {
            await target();
        }
        catch (Exception exception)
        {
            return await Verify(exception);
        }

        throw new("Did not throw.");
    }

    public async Task<VerifyResult> ThrowsTask(Func<Task> target)
    {
        ScrubInnerVerifier();
        try
        {
            await target();
        }
        catch (Exception exception)
        {
            return await Verify(exception);
        }

        throw new("Did not throw.");
    }

    public async Task<VerifyResult> ThrowsTask<T>(Func<Task<T>> target)
    {
        ScrubInnerVerifier();
        try
        {
            await target();
        }
        catch (Exception exception)
        {
            return await Verify(exception);
        }

        throw new("Did not throw.");
    }
}