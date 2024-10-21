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

    [Fact]
    public Task Two()
    {
        List<string> a = ["A", "b", "C"];
        List<int> b = [1, 2, 3];
        return VerifyCombinations(
            (a, b) => a.ToLower() + b,
            a, b);
    }
}