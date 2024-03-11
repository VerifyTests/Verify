public class AutoVerify
{
    [Fact]
    public async Task Simple()
    {
        var path = CurrentFile.Relative("AutoVerify.Simple.verified.txt");
        var fullPath = Path.GetFullPath(path);
        File.Delete(fullPath);
        await Verify("Foo")
            .AutoVerify();
        File.Delete(fullPath);
    }

    [Fact]
    public async Task DelegateTrue()
    {
        var funcCalled = false;
        var path = CurrentFile.Relative("AutoVerify.DelegateTrue.verified.txt");
        var fullPath = Path.GetFullPath(path);
        File.Delete(fullPath);
        await Verify("Foo")
            .AutoVerify(
                verifiedFile =>
                {
                    Assert.NotNull(verifiedFile);
                    funcCalled = true;
                    return true;
                });
        Assert.True(funcCalled);
        File.Delete(fullPath);
    }

    [Fact]
    public async Task DelegateTrueCustomNames()
    {
        var funcCalled = false;
        var path = CurrentFile.Relative("diffTypeNameTrue.diffMethodName.verified.txt");
        var fullPath = Path.GetFullPath(path);
        File.Delete(fullPath);
        await Verify("Foo")
            .UseTypeName("diffTypeNameTrue")
            .UseMethodName("diffMethodName")
            .AutoVerify(
                verifiedFile =>
                {
                    Assert.NotNull(verifiedFile);
                    funcCalled = true;
                    return true;
                });
        Assert.True(funcCalled);
        File.Delete(fullPath);
    }

    [Fact]
    public async Task DelegateFalse()
    {
        var funcCalled = false;
        var path = CurrentFile.Relative("AutoVerify.DelegateFalse.verified.txt");
        var fullPath = Path.GetFullPath(path);
        File.Delete(fullPath);
        await Assert.ThrowsAsync<VerifyException>(
            () => Verify("Foo")
                .DisableDiff()
                .AutoVerify(
                    verifiedFile =>
                    {
                        Assert.NotNull(verifiedFile);
                        funcCalled = true;
                        return false;
                    }));
        Assert.True(funcCalled);
        File.Delete(fullPath);
    }

    [Fact]
    public async Task DelegateFalseCustomNames()
    {
        var funcCalled = false;
        var path = CurrentFile.Relative("diffTypeNameFalse.diffMethodName.verified.txt");
        var fullPath = Path.GetFullPath(path);
        File.Delete(fullPath);
        await Assert.ThrowsAsync<VerifyException>(
            () => Verify("Foo")
                .DisableDiff()
                .UseTypeName("diffTypeNameFalse")
                .UseMethodName("diffMethodName")
                .AutoVerify(
                    verifiedFile =>
                    {
                        Assert.NotNull(verifiedFile);
                        funcCalled = true;
                        return false;
                    }));
        Assert.True(funcCalled);
        File.Delete(fullPath);
    }
}