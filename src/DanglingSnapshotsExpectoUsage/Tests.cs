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
    public static Test Second = Runner.TestCase(
        nameof(Second),
        () => Verify(
            name: nameof(Second),
            target: "Foo"));
}
