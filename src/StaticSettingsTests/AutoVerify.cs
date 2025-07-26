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
        var settings = new VerifySettings();
        settings.OnVerifyMismatch((pair, _, autoVerify) =>
        {
            Assert.True(File.Exists(pair.ReceivedPath));
            Assert.True(File.Exists(pair.VerifiedPath));
            Assert.True(autoVerify);
            return Task.CompletedTask;
        });
        try
        {
            await Verify("Foo", settings);
        }
        finally
        {
            File.Delete(fullPath);
        }
    }

    [Fact]
    public async Task DelegateTrue()
    {
        var funcCalled = false;
        VerifierSettings.AutoVerify((typeName, methodName, verifiedFile) =>
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
        var settings = new VerifySettings();
        settings.OnVerifyMismatch((pair, _, autoVerify) =>
        {
            Assert.True(File.Exists(pair.ReceivedPath));
            Assert.True(File.Exists(pair.VerifiedPath));
            Assert.True(autoVerify);
            return Task.CompletedTask;
        });
        try
        {
            await Verify("Foo", settings);
            Assert.True(funcCalled);
        }
        finally
        {
            File.Delete(fullPath);
        }
    }

    [Fact]
    public async Task DelegateTrueCustomNames()
    {
        var funcCalled = false;
        VerifierSettings.AutoVerify((typeName, methodName, verifiedFile) =>
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
        try
        {
            await Verify("Foo")
                .UseTypeName("diffTypeNameTrue")
                .UseMethodName("diffMethodName");
            Assert.True(funcCalled);
        }
        finally
        {
            File.Delete(fullPath);
        }
    }

    [Fact]
    public async Task DelegateFalse()
    {
        var funcCalled = false;
        VerifierSettings.AutoVerify((typeName, methodName, verifiedFile) =>
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
        try
        {
            await Assert.ThrowsAsync<VerifyException>(() => Verify("Foo")
                .DisableDiff());
            Assert.False(File.Exists(fullPath));
            Assert.True(funcCalled);
        }
        finally
        {

            File.Delete(fullPath);
        }
    }

    [Fact]
    public async Task DelegateFalseCustomNames()
    {
        var funcCalled = false;
        VerifierSettings.AutoVerify((typeName, methodName, verifiedFile) =>
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
        try
        {
            await Assert.ThrowsAsync<VerifyException>(() => Verify("Foo")
                .DisableDiff()
                .UseTypeName("diffTypeNameFalse")
                .UseMethodName("diffMethodName"));
            Assert.False(File.Exists(fullPath));
            Assert.True(funcCalled);
        }
        finally
        {
            File.Delete(fullPath);
        }
    }

    [Fact]
    public async Task IncludeBuildserver()
    {
        DiffEngine.BuildServerDetector.Detected = true;

        var path = CurrentFile.Relative("AutoVerify.IncludeBuildserver.verified.txt");
        var fullPath = Path.GetFullPath(path);
        File.Delete(fullPath);
        try
        {
            var result = await Verify("Hello")
                .AutoVerify(includeBuildServer: true, throwException: false);

            Assert.True(File.Exists(result.Files.First()));
        }
        finally
        {
            File.Delete(fullPath);
        }
    }

    [Fact]
    public async Task ExcludeBuildserver()
    {
        DiffEngine.BuildServerDetector.Detected = true;
        var path = CurrentFile.Relative("AutoVerify.ExcludeBuildserver.verified.txt");
        var fullPath = Path.GetFullPath(path);
        File.Delete(fullPath);
        try
        {
            var result = async () => await Verify("Hello")
                .AutoVerify(includeBuildServer: false, throwException: false);

            await Assert.ThrowsAnyAsync<Exception>(result);
        }
        finally
        {
            File.Delete(fullPath);
        }
    }
}