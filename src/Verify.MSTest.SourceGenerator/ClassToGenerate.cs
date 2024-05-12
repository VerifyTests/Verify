namespace Verify.MSTest.SourceGenerator;

readonly record struct ClassToGenerate
{
    public string? Namespace { get; }
    public string ClassName { get; }
    public IReadOnlyCollection<ParentClass> ParentClasses { get; }

    public ClassToGenerate(string? @namespace, string className, IReadOnlyCollection<ParentClass> parentClasses)
    {
        Namespace = @namespace;
        ClassName = className;
        ParentClasses = parentClasses;
    }
}
