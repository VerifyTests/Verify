public class Tests
{
    [Tests]
    public static Test myTest = Runner.TestCase(
        "myTest",
        () => Verify(
            name: nameof(myTest),
            target: "value"));

    [Tests]
    public static Test withTargets = Runner.TestCase(
        "withTargets",
        () => Verify(
            nameof(withTargets), "value"));
}