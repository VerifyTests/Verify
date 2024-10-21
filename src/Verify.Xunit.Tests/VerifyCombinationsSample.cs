public class VerifyCombinationsSample
{
    [Fact]
    public Task One() =>
        VerifyCombinations<string>(
            _ => _.ToLower(),
            ["A", "b", "C"]);
}