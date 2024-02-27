public class CounterTarget
{
    public string? Property1 { get; set; }
    public string? Property2 { get; set; }
    public string? Property3 { get; set; }
}

public class CustomCounterTests
{
    static AsyncLocal<List<string>?> local = new();

    [ModuleInitializer]
    public static void CounterInit()
    {
        VerifierSettings.OnVerify(
            before: () => local.Value = new(),
            after: () => local.Value = null
        );

        AddCountConverter<CounterTarget>( _ => _.Property1);
        AddCountConverter<CounterTarget>( _ => _.Property2);
        AddCountConverter<CounterTarget>( _ => _.Property3);
    }

    static void AddCountConverter<T>(Expression<Func<T, string?>> expression) =>
        VerifierSettings.MemberConverter<T, string?>(
            expression: expression,
            converter: ConvertMember);

    static string? ConvertMember(string? member)
    {
        if (member == null)
        {
            return null;
        }

        var values = local.Value!;
        var index = values.IndexOf(member);

        if (index == -1)
        {
            index = values.Count;
            values.Add(member);
        }

        return $"number{index + 1}";
    }

    [Fact]
    public Task CounterUsage()
    {
        var target = new CounterTarget
        {
            Property1 = "valueA",
            Property2 = "valueB",
            Property3 = "valueA",
        };
        return Verify(target);
    }
}