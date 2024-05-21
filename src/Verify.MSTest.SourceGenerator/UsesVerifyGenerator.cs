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
                transform: GetSemanticTargetForGeneration)
            .Where(static classToGenerate => classToGenerate is not null)
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

                    if (!IsAssemblyEligibleForGeneration(context.SemanticModel.Compilation, cancel))
                    {
                        return null;
                    }

                    if (context.SemanticModel.GetDeclaredSymbol(syntax) is not INamedTypeSymbol symbol)
                    {
                        return null;
                    }

                    cancel.ThrowIfCancellationRequested();

                    if (!symbol.HasAttributeThatInheritsFrom(TestClassAttributeName))
                    {
                        return null;
                    }

                    var x = symbol.BaseType;
                    var result = false;
                    while (x is not null)
                    {
                        if (x.HasAttributeThatInheritsFrom(TestClassAttributeName))
                        {
                            result = true;
                            break;
                        }

                        x = x.BaseType;
                    }
                    if (result)
                    {
                        return null;
                    }

                    cancel.ThrowIfCancellationRequested();

                    return Parser.Parse(symbol, syntax, cancel);
                })
            .Where(static classToGenerate => classToGenerate is not null)
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

    static bool IsAssemblyEligibleForGeneration(Compilation compilation, Cancel _) => compilation.Assembly.HasAttribute(MarkerAttributeName);

    // TODO: Either inline or pull the assembly attribute logic into this method.
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

        // Only generate for classes that won't get one defined by another attribute.
        if (symbol.HasAttributeOnBaseTypes(MarkerAttributeName))
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
