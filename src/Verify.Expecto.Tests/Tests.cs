public class Tests
{
    [Tests]
    public static Test tests = Runner.TestCase(
        "myTest",
        () => Verifier.Verify("myTest", "value"));
}