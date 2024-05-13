namespace VerifyMSTest.SourceGenerator;

readonly record struct ParentClass
{
    public string Keyword { get; }
    public string Name { get; }

    public ParentClass(string keyword, string name)
    {
        Keyword = keyword;
        Name = name;
    }
}
