public class RegisterFileConverterTests :
    BaseTest
{
    [Fact]
    public void ObjectThrows()
    {
        var exception = Assert.Throws<Exception>(
            () => VerifierSettings.RegisterFileConverter<object>(
                (_, _) => new(null, "txt", "value")));
        Assert.Contains("too greedy", exception.Message);
    }

    [Fact]
    public void ObjectAsyncThrows()
    {
        var exception = Assert.Throws<Exception>(
            () => VerifierSettings.RegisterFileConverter<object>(
                (_, _) => Task.FromResult<ConversionResult>(new(null, "txt", "value"))));
        Assert.Contains("too greedy", exception.Message);
    }
}
