namespace VerifyMSTest.SourceGenerator;

[Generator]
public class UsesVerifyGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classesToGenerate = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: Parser.MarkerAttributeName,
                predicate: IsSyntaxEligibleForGeneration,
                transform: GetSemanticTargetForGeneration)
            .WithTrackingName(TrackingNames.InitialTransform)
            .Where(static classToGenerate => classToGenerate is not null)
            .WithTrackingName(TrackingNames.RemoveNulls);

        // Collect the classes to generate into a collection so that we can write them
        // to a single file and avoid the issues of ambiguous hint names discussed in
        // https://github.com/dotnet/roslyn/discussions/60272.
        var classesCollection = classesToGenerate
            .Collect()
            .WithTrackingName(TrackingNames.Collect);

        context.RegisterSourceOutput(classesCollection, Execute);
    }

    static bool IsSyntaxEligibleForGeneration(SyntaxNode node, Cancel _) => node is ClassDeclarationSyntax;

    static ClassToGenerate? GetSemanticTargetForGeneration(GeneratorAttributeSyntaxContext context, Cancel cancel)
    {
        if (context.TargetSymbol is not INamedTypeSymbol symbol)
        {
            return null;
        }

        if (context.TargetNode is not TypeDeclarationSyntax syntax)
        {
            return null;
        }

        cancel.ThrowIfCancellationRequested();

        return Parser.Parse(symbol, syntax, cancel);
    }

    static void Execute(SourceProductionContext context, ImmutableArray<ClassToGenerate?> classesToGenerate)
    {
        if (classesToGenerate.IsDefaultOrEmpty)
        {
            return;
        }

        var classes = classesToGenerate.Distinct().OfType<ClassToGenerate>().ToList();
        if (classes.Count == 0)
        {
            return;
        }

        var cancel = context.CancellationToken;
        cancel.ThrowIfCancellationRequested();

        var emitter = new Emitter();
        var sourceCode = emitter.GenerateExtensionClasses(classes, cancel);
        context.AddSource("UsesVerify.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
    }
}
