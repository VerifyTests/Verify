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
        Assert.True(File.Exists(fullPath));
        File.Delete(fullPath);
    }

    [Fact]
    public async Task DelegateTrue()
    {
        var funcCalled = false;
        VerifierSettings.AutoVerify(
            (typeName, methodName, file) =>
            {
                Assert.NotNull(typeName);
                Assert.NotNull(methodName);
                Assert.NotNull(file);
                funcCalled = true;
                return true;
            });
        var path = CurrentFile.Relative("AutoVerify.DelegateTrue.verified.txt");
        var fullPath = Path.GetFullPath(path);
        File.Delete(fullPath);
        await Verify("Foo");
        Assert.True(File.Exists(fullPath));
        Assert.True(funcCalled);
        File.Delete(fullPath);
    }

    [Fact]
    public async Task DelegateFalse()
    {
        var funcCalled = false;
        VerifierSettings.AutoVerify(
            (typeName, methodName, file) =>
            {
                Assert.NotNull(typeName);
                Assert.NotNull(methodName);
                Assert.NotNull(file);
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
}