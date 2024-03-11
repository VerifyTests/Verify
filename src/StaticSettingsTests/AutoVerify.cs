public class AutoVerify :
    BaseTest
{
    [Fact]
    public async Task Simple()
    {
        VerifierSettings.AutoVerify();
        var path = CurrentFile.Relative("AutoVerify.Simple.verified.txt");
        var fullPath = Path.GetFullPath(path);
        File.Delete(fullPath);
        await Verify("Foo");
        File.Delete(fullPath);
    }

    [Fact]
    public async Task DelegateTrue()
    {
        var funcCalled = false;
        VerifierSettings.AutoVerify(
            (typeName, methodName, verifiedFile) =>
            {
                Assert.NotNull(typeName);
                Assert.NotNull(methodName);
                Assert.NotNull(verifiedFile);
                funcCalled = true;
                return true;
            });
        var path = CurrentFile.Relative("AutoVerify.DelegateTrue.verified.txt");
        var fullPath = Path.GetFullPath(path);
        File.Delete(fullPath);
        await Verify("Foo");
        Assert.True(funcCalled);
        File.Delete(fullPath);
    }

    [Fact]
    public async Task DelegateTrueCustomNames()
    {
        var funcCalled = false;
        VerifierSettings.AutoVerify(
            (typeName, methodName, verifiedFile) =>
            {
                Assert.Equal("diffTypeNameTrue", typeName);
                Assert.Equal("diffMethodName", methodName);
                Assert.NotNull(verifiedFile);
                funcCalled = true;
                return true;
            });
        var path = CurrentFile.Relative("diffTypeNameTrue.diffMethodName.verified.txt");
        var fullPath = Path.GetFullPath(path);
        File.Delete(fullPath);
        await Verify("Foo")
            .UseTypeName("diffTypeNameTrue")
            .UseMethodName("diffMethodName");
        Assert.True(funcCalled);
        File.Delete(fullPath);
    }

    [Fact]
    public async Task DelegateFalse()
    {
        var funcCalled = false;
        VerifierSettings.AutoVerify(
            (typeName, methodName, verifiedFile) =>
            {
                Assert.NotNull(typeName);
                Assert.NotNull(methodName);
                Assert.NotNull(verifiedFile);
                funcCalled = true;
                return false;
            });
        var path = CurrentFile.Relative("AutoVerify.DelegateFalse.verified.txt");
        var fullPath = Path.GetFullPath(path);
        File.Delete(fullPath);
        await Assert.ThrowsAsync<VerifyException>(
            () => Verify("Foo")
                .DisableDiff());
        Assert.False(File.Exists(fullPath));
        Assert.True(funcCalled);
        File.Delete(fullPath);
    }

    [Fact]
    public async Task DelegateFalseCustomNames()
    {
        var funcCalled = false;
        VerifierSettings.AutoVerify(
            (typeName, methodName, verifiedFile) =>
            {
                Assert.Equal("diffTypeNameFalse", typeName);
                Assert.Equal("diffMethodName", methodName);
                Assert.NotNull(verifiedFile);
                funcCalled = true;
                return false;
            });
        var path = CurrentFile.Relative("diffTypeNameFalse.diffMethodName.verified.txt");
        var fullPath = Path.GetFullPath(path);
        File.Delete(fullPath);
        await Assert.ThrowsAsync<VerifyException>(
            () => Verify("Foo")
                .DisableDiff()
                .UseTypeName("diffTypeNameFalse")
                .UseMethodName("diffMethodName"));
        Assert.False(File.Exists(fullPath));
        Assert.True(funcCalled);
        File.Delete(fullPath);
    }
}