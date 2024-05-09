namespace Verify.MSTest.SourceGenerator;

internal class ParentClass
{
    public ParentClass(string keyword, string name, string constraints, ParentClass? child)
    {
        Keyword = keyword;
        Name = name;
        Constraints = constraints;
        Child = child;
    }

    public ParentClass? Child { get; }
    public string Keyword { get; }
    public string Name { get; }
    public string Constraints { get; }
}