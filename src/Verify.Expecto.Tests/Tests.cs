public class Tests
{
    [Tests]
    public static Test myTest = Runner.TestCase(
        "myTest",
        () => Verify(
            name: nameof(myTest),
            target: "value"));
}