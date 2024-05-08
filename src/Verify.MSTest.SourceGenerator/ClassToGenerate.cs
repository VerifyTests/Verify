namespace Verify.MSTest.SourceGenerator;

readonly record struct ClassToGenerate
{
    public readonly string Namespace;
    public readonly string ClassName;

    public ClassToGenerate(string @namespace, string className)
    {
        Namespace = @namespace;
        ClassName = className;
    }
}
