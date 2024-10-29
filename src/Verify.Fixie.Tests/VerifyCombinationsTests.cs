﻿public class VerifyCombinationsTests
{
    public Task One()
    {
        string[] list = ["A", "b", "C"];
        return Combination()
            .Verify(
                _ => _.ToLower(),
                list);
    }

    public Task KeysWithInvalidPathChars()
    {
        string[] list = ["/", "\\"];
        return Combination()
            .Verify(
                _ => _.ToLower(),
                list);
    }

    public Task Two()
    {
        string[] a = ["A", "b", "C"];
        int[] b = [1, 2, 3];
        return Combination()
            .Verify(
                (a, b) => a.ToLower() + b,
                a, b);
    }

    public Task WithScrubbed()
    {
        int[] years = [2020, 2022];
        int[] months = [2, 3];
        int[] dates = [12, 15];
        return Combination()
            .Verify(
                (year, month, date) => new DateTime(year, month, date),
                years, months, dates);
    }

    public Task WithDontScrub()
    {
        int[] years = [2020, 2022];
        int[] months = [2, 3];
        int[] dates = [12, 15];
        return Combination()
            .Verify(
                (year, month, date) => new DateTime(year, month, date),
                years, months, dates)
            .DontScrubDateTimes();
    }

    public Task Three()
    {
        string[] a = ["A", "b", "C"];
        int[] b = [1, 2, 3];
        bool[] c = [true, false];
        return Combination()
            .Verify(
                (a, b, c) => a.ToLower() + b + c,
                a, b, c);
    }

    public Task MixedLengths()
    {
        string[] a = ["A", "bcc", "sssssC"];
        int[] b = [100, 2, 30];
        bool[] c = [true, false];
        return Combination()
            .Verify(
                (a, b, c) => a.ToLower() + b + c,
                a, b, c);
    }

    public Task WithException()
    {
        string[] a = ["A", "b", "C"];
        int[] b = [1, 2, 3];
        bool[] c = [true, false];
        return Combination(captureExceptions: true)
            .Verify(
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
}