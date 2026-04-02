// ReSharper disable ArrangeStaticMemberQualifier
// ReSharper disable UnusedParameter.Local
public class DerivePathInfoTests :
    BaseTest
{
    [Fact]
    public Task OmitTypeName()
    {
        #region DerivePathInfoOmitTypeName

        Verifier.DerivePathInfo(
            (sourceFile, projectDirectory, type, method) =>
            {
                var directory = Path.GetDirectoryName(sourceFile)!;
                return new(
                    directory: Path.Combine(directory, type.NameWithParent()),
                    typeName: string.Empty,
                    methodName: method.Name);
            });

        #endregion

        return Verify("value");
    }
}
