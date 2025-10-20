#region NUnitExtensionSample

[TestFixture]
public class ExtensionSample
{
    [Test]
    public Task Method() =>
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