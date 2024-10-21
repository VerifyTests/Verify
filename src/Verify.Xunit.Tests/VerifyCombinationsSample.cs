public class VerifyCombinationsSample
{
    [Fact]
    public Task One()
    {
        List<string> list = ["A", "b", "C"];
        return VerifyCombinations(
            _ => _.ToLower(),
            list);
    }
}