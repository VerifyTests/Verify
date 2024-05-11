namespace Verify.MSTest.SourceGenerator;

internal class ParentClass
{
    public ParentClass(string keyword, string name, ParentClass? child)
    {
        Keyword = keyword;
        Name = name;
        Child = child;
    }

    public ParentClass? Child { get; }
    public string Keyword { get; }
    public string Name { get; }
}