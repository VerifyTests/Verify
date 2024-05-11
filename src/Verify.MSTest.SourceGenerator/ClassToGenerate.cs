using System.Collections.Immutable;

namespace Verify.MSTest.SourceGenerator;

readonly record struct ClassToGenerate
{
    public readonly string? Namespace;
    public readonly string ClassName;
    public readonly IReadOnlyCollection<string> TypeParameters;
    public readonly ParentClass? ParentClass;

    public ClassToGenerate(string? @namespace, string className, IReadOnlyCollection<string> typeParameters, ParentClass? parentClass)
    {
        Namespace = @namespace;
        ClassName = className;
        TypeParameters = typeParameters;
        ParentClass = parentClass;
    }
}
