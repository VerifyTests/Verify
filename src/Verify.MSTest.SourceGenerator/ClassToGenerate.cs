namespace Verify.MSTest.SourceGenerator;

readonly record struct ClassToGenerate
{
    public string? Namespace { get; }
    public string ClassName { get; }
    public IReadOnlyCollection<string> TypeParameters { get; }
    public IReadOnlyCollection<ParentClass> ParentClasses { get; }

    public ClassToGenerate(string? @namespace, string className, IReadOnlyCollection<string> typeParameters, IReadOnlyCollection<ParentClass> parentClasses)
    {
        Namespace = @namespace;
        ClassName = className;
        TypeParameters = typeParameters;
        ParentClasses = parentClasses;
    }
}
