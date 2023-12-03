[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class TestCase(params object[] parameters) :
    Attribute
{
    public object[] Parameters { get; } = parameters;
}