namespace TheTests;

#region MSTestExtensionSample

[TestClass]
public class ExtensionSample :
    VerifyBase
{
    [TestMethod]
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

    [TestMethod]
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