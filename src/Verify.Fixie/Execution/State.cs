namespace VerifyFixie;

public class State(TestClass testClass, Test test, object[]? parameters)
{
    public TestClass TestClass { get; } = testClass;
    public Test Test { get; } = test;
    public object[]? Parameters { get; } = parameters;
}