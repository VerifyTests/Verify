namespace VerifyMSTest.SourceGenerator;

readonly record struct ClassToGenerate
{
    public string? Namespace { get; }
    public string ClassName { get; }
    public EquatableArray<ParentClass> ParentClasses { get; }

    public ClassToGenerate(string? @namespace, string className, ParentClass[] parentClasses)
    {
        Namespace = @namespace;
        ClassName = className;
        ParentClasses = new EquatableArray<ParentClass>(parentClasses);
    }
}
