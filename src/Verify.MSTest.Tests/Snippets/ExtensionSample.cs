#region MSTestExtensionSample

[TestClass]
public partial class ExtensionSample
{
    [TestMethod]
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