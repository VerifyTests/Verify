public class VerifyCombinationsTests
{
    [Test]
    public Task One()
    {
        List<string> list = ["A", "b", "C"];
        return VerifyCombinations(
            _ => _.ToLower(),
            list);
    }

    [Test]
    public Task KeysWithInvalidPathChars()
    {
        List<string> list = ["/", "\\"];
        return VerifyCombinations(
            _ => _.ToLower(),
            list);
    }

    [Test]
    public Task Two()
    {
        List<string> a = ["A", "b", "C"];
        List<int> b = [1, 2, 3];
        return VerifyCombinations(
            (a, b) => a.ToLower() + b,
            a, b);
    }

    [Test]
    public Task WithScrubbed()
    {
        List<int> years = [2020, 2022];
        List<int> months = [2, 3];
        List<int> dates = [12, 15];
        return VerifyCombinations(
            (year, month, date) => new DateTime(year, month, date),
            years, months, dates);
    }

    [Test]
    public Task WithDontScrub()
    {
        List<int> years = [2020, 2022];
        List<int> months = [2, 3];
        List<int> dates = [12, 15];
        return VerifyCombinations(
            (year, month, date) => new DateTime(year, month, date),
            years, months, dates)
            .DontScrubDateTimes();
    }

    [Test]
    public Task Three()
    {
        List<string> a = ["A", "b", "C"];
        List<int> b = [1, 2, 3];
        List<bool> c = [true, false];
        return VerifyCombinations(
            (a, b, c) => a.ToLower() + b + c,
            a, b, c);
    }

    [Test]
    public Task MixedLengths()
    {
        List<string> a = ["A", "bcc", "sssssC"];
        List<int> b = [100, 2, 30];
        List<bool> c = [true, false];
        return VerifyCombinations(
            (a, b, c) => a.ToLower() + b + c,
            a, b, c);
    }

    [Test]
    public Task WithException()
    {
        List<string> a = ["A", "b", "C"];
        List<int> b = [1, 2, 3];
        List<bool> c = [true, false];
        return VerifyCombinations(
            (a, b, c) =>
            {
                if (a == "b")
                {
                    throw new ArgumentException("B is not allowed");
                }

                return a.ToLower() + b + c;
            },
            a, b, c);
    }

    [Test]
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