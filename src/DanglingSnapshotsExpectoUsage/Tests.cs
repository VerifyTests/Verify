using Expecto;

public class Tests
{
    [Tests]
    public static Test Simple = Runner.TestCase(
        nameof(Simple),
        () => Verify(
            name: nameof(Simple),
            target: "Foo"));

    [Tests]
    public static Test IncorrectCase = Runner.TestCase(
        nameof(IncorrectCase),
        () => Verify(
            name: nameof(IncorrectCase),
            target: "Foo"));
}
