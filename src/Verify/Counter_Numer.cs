namespace VerifyTests;

public partial class Counter
{
    Dictionary<long, (int intValue, string stringValue)> numberCache = [];
    static Dictionary<long, string> globalNamedNumbers = [];
    int currentNumber;

    public int Next(int input) =>
        NextValue(input)
            .intValue;

    public int Next(long input) =>
        NextValue(input)
            .intValue;

    internal static void AddNamed(int value, string name)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        globalNamedNumbers.Add(value, name);
    }

    internal static void AddNamed(long value, string name)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        globalNamedNumbers.Add(value, name);
    }

    public string NextString(int input) =>
        NextValue(input)
            .stringValue;

    public string NextString(long input) =>
        NextValue(input)
            .stringValue;

    (int intValue, string stringValue) NextValue(long input)
    {
        if (namedNumbers.TryGetValue(input, out var name) ||
            globalNamedNumbers.TryGetValue(input, out name))
        {
            return new(0, name);
        }

        return numberCache.GetOrAdd(
            input,
            _ => BuildNumberValue());
    }

    (int intValue, string stringValue) NextValue(int input) =>
        NextValue((long) input);

    (int intValue, string stringValue) BuildNumberValue()
    {
        var value = Interlocked.Increment(ref currentNumber);
        return (value, $"Number_{value}");
    }
}