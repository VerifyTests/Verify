[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class TestCase(params object[] parameters) :
    Attribute
{
    public object[] Parameters { get; } = parameters;
}