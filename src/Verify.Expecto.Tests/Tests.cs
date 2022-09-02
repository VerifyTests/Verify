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
            name: nameof(withTargets),
            target: new
            {
                Property = "Value"
            },
            rawTargets: new[]
            {
                new Target("txt", "TextTarget")
            }));
}