namespace VerifyTests;

public class CombinationResults(IReadOnlyList<CombinationResult> items, IReadOnlyList<Type> keyTypes)
{
    public IReadOnlyList<CombinationResult> Items { get; } = items;
    public IReadOnlyList<Type> KeyTypes { get; } = keyTypes;
}