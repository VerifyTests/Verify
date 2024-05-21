class TestDriver(IEnumerable<ISourceGenerator> sourceGenerators)
{
    public GeneratorDriverResults Run(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        PortableExecutableReference[] references =
        [
            MetadataReference.CreateFromFile(typeof(VerifyMSTest.UsesVerifyAttribute).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute).Assembly.Location),
        ];

        var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: [syntaxTree],
            references: references);

        var driverOptions = new GeneratorDriverOptions(
            disabledOutputs: IncrementalGeneratorOutputKind.None,
            trackIncrementalGeneratorSteps: true);

        GeneratorDriver driver = CSharpGeneratorDriver.Create(sourceGenerators, driverOptions: driverOptions);

        driver = driver.RunGenerators(compilation);
        var results1 = driver.GetRunResult();
        var timings1 = driver.GetTimingInfo();
        driver = driver.RunGenerators(compilation.Clone());
        var results2 = driver.GetRunResult();
        var timings2 = driver.GetTimingInfo();

        return new(
            new(results1, timings1),
            new(results2, timings2));
    }
}
