class TestDriver(IEnumerable<ISourceGenerator> sourceGenerators)
{
    public GeneratorDriverResults Run(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        // Collect assembly references for types like `System.Object` and add the types used by our tests.
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
            .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
            .Concat(
            [
                MetadataReference.CreateFromFile(typeof(VerifyMSTest.UsesVerifyAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute).Assembly.Location),
            ]);

        var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            .WithSpecificDiagnosticOptions(new Dictionary<string, ReportDiagnostic>
            {
                // Suppress "CS1701: Assuming assembly reference '...' matches '...', you may need to supply runtime policy."
                // This is already suppressed by the SDK, but because we're directly invoking the compiler ourselves we need to
                // do it here. See https://github.com/dotnet/roslyn/issues/19640.
                ["CS1701"] = ReportDiagnostic.Suppress,
                ["CS1702"] = ReportDiagnostic.Suppress,
            });

        var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: [syntaxTree],
            references: references,
            options: compilationOptions);

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
