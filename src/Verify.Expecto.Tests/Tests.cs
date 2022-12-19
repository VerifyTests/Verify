public class Tests
{
    [Tests]
    public static Test myTest = Runner.TestCase(
        nameof(myTest),
        () => Verify(
            name: nameof(myTest),
            target: "value"));

    [Tests]
    public static Test stringTarget = Runner.TestCase(
        nameof(stringTarget),
        () => Verify(
            name: nameof(stringTarget),
            target: new Target("txt", "Value")));

    [Tests]
    public static Test withTargets = Runner.TestCase(
        nameof(withTargets),
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