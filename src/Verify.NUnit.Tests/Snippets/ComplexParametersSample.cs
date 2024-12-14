#region NameForParametersNunit

public class ComplexParametersSample
{
    [ModuleInitializer]
    public static void Initialize() =>
        VerifierSettings.NameForParameter<ComplexData>(_ => _.Value);

    [TestCaseSource(nameof(GetData))]
    public Task ComplexTestCaseSource(ComplexData arg) =>
        Verify(arg);

    public static IEnumerable<object[]> GetData()
    {
        yield return
        [
            new ComplexData("Value1")
        ];
        yield return
        [
            new ComplexData("Value2")
        ];
    }

    public record ComplexData(string Value);
}

#endregion