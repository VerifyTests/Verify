namespace VerifyTests;

public class CombinationResults(IReadOnlyList<CombinationResult> items, IReadOnlyList<Type> keyTypes, IReadOnlyList<string>? columns)
{
    public IReadOnlyList<string>? Columns { get; } = columns;
    public IReadOnlyList<CombinationResult> Items { get; } = items;
    public IReadOnlyList<Type> KeyTypes { get; } = keyTypes;
}