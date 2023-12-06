#region NUnitExtensionSample

[TestFixture]
public class ExtensionSample
{
    [Test]
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

    [Test]
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