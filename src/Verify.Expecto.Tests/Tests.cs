public class Tests
{
    [Tests] public static Test tests = Runner.TestCase(
        "myTest",
        () => Verify("myTest", "value"));
}