namespace VerifyMSTest.SourceGenerator;

[Generator]
public class UsesVerifyGenerator : IIncrementalGenerator
{
    static string MarkerAttributeName => "VerifyMSTest.UsesVerifyAttribute";
    static string TestClassAttributeName => "Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var markerAttributeClassesToGenerate = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: MarkerAttributeName,
                predicate: IsSyntaxEligibleForGeneration,
                transform: static (context, cancel) =>
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

                    // Only run generator for classes when the parent won't _also_ have generation.
                    // Otherwise the generator will hide the base member.
                    if (HasParentWithMarkerAttribute(symbol))
                    {
                        return null;
                    }

                    cancel.ThrowIfCancellationRequested();

                    return Parser.Parse(symbol, syntax, cancel);
                })
            .WhereNotNull()
            .WithTrackingName(TrackingNames.MarkerAttributeInitialTransform)
            .Collect();

        var assemblyAttributeClassesToGenerate = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: IsSyntaxEligibleForGeneration,
                transform: static (context, cancel) =>
                {
                    if (context.Node is not TypeDeclarationSyntax syntax)
                    {
                        return null;
                    }

                    if (!IsAssemblyEligibleForGeneration(context.SemanticModel.Compilation.Assembly))
                    {
                        return null;
                    }

                    if (context.SemanticModel.GetDeclaredSymbol(syntax, cancel) is not INamedTypeSymbol symbol)
                    {
                        return null;
                    }

                    cancel.ThrowIfCancellationRequested();

                    if (!symbol.HasAttributeOfType(TestClassAttributeName, includeDerived: true))
                    {
                        return null;
                    }

                    // Only run generator for classes when the parent won't _also_ have generation.
                    // Otherwise the generator will hide the base member.
                    if (HasParentWithTestClassAttribute(symbol) || HasParentWithMarkerAttribute(symbol))
                    {
                        return null;
                    }

                    cancel.ThrowIfCancellationRequested();

                    return Parser.Parse(symbol, syntax, cancel);
                })
            .WhereNotNull()
            .WithTrackingName(TrackingNames.AssemblyAttributeInitialTransform)
            .Collect();

        // Collect the classes to generate into a single collection so that we can write them to a single file and
        // avoid the issues of ambiguous hint names discussed in https://github.com/dotnet/roslyn/discussions/60272.
        var classesToGenerate = markerAttributeClassesToGenerate.Combine(assemblyAttributeClassesToGenerate)
            .SelectMany((classes, _) => classes.Left.AddRange(classes.Right))
            .WithTrackingName(TrackingNames.Merge)
            .Collect()
            .WithTrackingName(TrackingNames.Complete);

        context.RegisterSourceOutput(classesToGenerate, Execute);
    }

    static bool IsSyntaxEligibleForGeneration(SyntaxNode node, Cancel _) => node is ClassDeclarationSyntax;

    static bool IsAssemblyEligibleForGeneration(IAssemblySymbol assembly) => assembly.HasAttributeOfType(MarkerAttributeName, includeDerived: false);

    static bool HasParentWithMarkerAttribute(INamedTypeSymbol symbol) => symbol
        .GetBaseTypes()
        .Any(parent => parent.HasAttributeOfType(MarkerAttributeName, includeDerived: false));

    static bool HasParentWithTestClassAttribute(INamedTypeSymbol symbol) => symbol
        .GetBaseTypes()
        .Any(parent => parent.HasAttributeOfType(TestClassAttributeName, includeDerived: true));

    static void Execute(SourceProductionContext context, ImmutableArray<ClassToGenerate> classesToGenerate)
    {
        if (classesToGenerate.IsDefaultOrEmpty)
        {
            return;
        }

        var classes = classesToGenerate.Distinct().ToList();

        var emitter = new Emitter();
        var sourceCode = emitter.GenerateExtensionClasses(classes, context.CancellationToken);
        context.AddSource("UsesVerify.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
    }
}
