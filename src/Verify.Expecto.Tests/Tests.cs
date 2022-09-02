public class Tests
{
    [Tests]
    public static Test tests = Runner.TestCase(
        "myTest",
        () => Verify("myTest", "value"));
    [Tests]
    public static Test withTargets = Runner.TestCase(
        "withTargets",
        () => Verify(nameof(withTargets), "value"));
}