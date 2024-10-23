[TestClass]
public partial class VerifyCombinationsTests
{
    [TestMethod]
    public Task One()
    {
        string[] list = ["A", "b", "C"];
        return VerifyCombinations(
            _ => _.ToLower(),
            list);
    }

    [TestMethod]
    public Task KeysWithInvalidPathChars()
    {
        string[] list = ["/", "\\"];
        return VerifyCombinations(
            _ => _.ToLower(),
            list);
    }

    [TestMethod]
    public Task Two()
    {
        string[] a = ["A", "b", "C"];
        int[] b = [1, 2, 3];
        return VerifyCombinations(
            (a, b) => a.ToLower() + b,
            a, b);
    }

    [TestMethod]
    public Task WithScrubbed()
    {
        int[] years = [2020, 2022];
        int[] months = [2, 3];
        int[] dates = [12, 15];
        return VerifyCombinations(
            (year, month, date) => new DateTime(year, month, date),
            years, months, dates);
    }

    [TestMethod]
    public Task WithDontScrub()
    {
        int[] years = [2020, 2022];
        int[] months = [2, 3];
        int[] dates = [12, 15];
        return VerifyCombinations(
            (year, month, date) => new DateTime(year, month, date),
            years, months, dates)
            .DontScrubDateTimes();
    }

    [TestMethod]
    public Task Three()
    {
        string[] a = ["A", "b", "C"];
        int[] b = [1, 2, 3];
        bool[] c = [true, false];
        return VerifyCombinations(
            (a, b, c) => a.ToLower() + b + c,
            a, b, c);
    }

    [TestMethod]
    public Task MixedLengths()
    {
        string[] a = ["A", "bcc", "sssssC"];
        int[] b = [100, 2, 30];
        bool[] c = [true, false];
        return VerifyCombinations(
            (a, b, c) => a.ToLower() + b + c,
            a, b, c);
    }

    [TestMethod]
    public Task WithException()
    {
        string[] a = ["A", "b", "C"];
        int[] b = [1, 2, 3];
        bool[] c = [true, false];
        return VerifyCombinations(
            (a, b, c) =>
            {
                if (a == "b")
                {
                    throw new ArgumentException("B is not allowed");
                }

                return a.ToLower() + b + c;
            },
            a, b, c,
            captureExceptions: true);
    }

    [TestMethod]
    public Task UnBound()
    {
        string[] a = ["A", "b", "C"];
        int[] b = [1, 2, 3];
        bool[] c = [true, false];
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