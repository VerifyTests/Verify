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
    [Fact]
    public Task Three()
    {
        List<string> a = ["A", "b", "C"];
        List<int> b = [1, 2, 3];
        List<bool> c = [true, false];
        return VerifyCombinations(
            (a, b, c) => a.ToLower() + b + c,
            a, b, c);
    }

    [Fact]
    public Task UnBound()
    {
        List<string> a = ["A", "b", "C"];
        List<int> b = [1, 2, 3];
        List<bool> c = [true, false];
        var list = new List<IEnumerable<object?>>
        {
            a.Cast<object?>(),
            b.Cast<object?>(),
            c.Cast<object?>()
        };
        return VerifyCombinations(
            _ =>
            {
                var a = (string)_[0]!;
                var b = (int)_[1]!;
                var c = (bool)_[2]!;
                return a.ToLower() + b + c;
            },
            list);
    }
}