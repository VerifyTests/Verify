class FrameworkNameVersion
{
    public string Name { get; }
    public string NameAndVersion { get; }

    public FrameworkNameVersion(string name, string nameAndVersion)
    {
        Name = name;
        NameAndVersion = nameAndVersion;
    }
}