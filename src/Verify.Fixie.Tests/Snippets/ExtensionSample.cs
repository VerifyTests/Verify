#region FixieExtensionSample

public class ExtensionSample
{
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