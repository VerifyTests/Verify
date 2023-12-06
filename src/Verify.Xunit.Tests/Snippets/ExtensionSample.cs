#region XunitExtensionSample

[UsesVerify]
public class ExtensionSample
{
    [Fact]
    public Task AtMethod() =>
        Verify(
            target: """
                    <note>
                      <to>Joe</to>
                      <from>Kim</from>
                      <heading>Reminder</heading>
                    </note>
                    """,
            extension: "xml");

    [Fact]
    public Task AtMethodFluent() =>
        Verify(
            target: """
                    <note>
                      <to>Joe</to>
                      <from>Kim</from>
                      <heading>Reminder</heading>
                    </note>
                    """,
            extension: "xml");
}

#endregion