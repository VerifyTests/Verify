namespace VerifyMSTest.SourceGenerator;

//https://github.com/dotnet/roslyn-analyzers/issues/6229
#pragma warning disable RS1036
[Generator]
public class UsesVerifyGenerator : IIncrementalGenerator
{
    static string MarkerAttributeName => "VerifyMSTest.UsesVerifyAttribute";
    static string TestClassAttributeName => "Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var markerAttributes = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: MarkerAttributeName,
                predicate: IsSyntaxEligibleForGeneration,
                transform: TransformClass)
            .WhereNotNull()
            .WithTrackingName(TrackingNames.MarkerAttributeInitialTransform)
            .Collect();

        var assemblyAttributes = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: IsSyntaxEligibleForGeneration,
                transform: TransformAssembly)
            .WhereNotNull()
            .WithTrackingName(TrackingNames.AssemblyAttributeInitialTransform)
            .Collect();

        // Collect the classes to generate into a single collection so that we can write them to a single file and
        // avoid the issues of ambiguous hint names discussed in https://github.com/dotnet/roslyn/discussions/60272.
        var toGenerate = markerAttributes.Combine(assemblyAttributes)
            .SelectMany((classes, _) => classes.Left.AddRange(classes.Right))
            .WithTrackingName(TrackingNames.Merge)
            .Collect()
            .WithTrackingName(TrackingNames.Complete);

        context.RegisterSourceOutput(toGenerate, Execute);
    }

    static ClassToGenerate? TransformClass(GeneratorAttributeSyntaxContext context, Cancel cancel)
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

        var markerType = context.SemanticModel.Compilation.GetTypeByMetadataName(MarkerAttributeName);
        if (markerType is null)
        {
            return null;
        }

        // Only run generator for classes when the parent won't _also_ have generation.
        // Otherwise the generator will hide the base member.
        if (HasParentWithMarkerAttribute(symbol, markerType))
        {
            return null;
        }

        return Parser.Parse(symbol, syntax, cancel);
    }

    static ClassToGenerate? TransformAssembly(GeneratorSyntaxContext context, Cancel cancel)
    {
        if (context.Node is not TypeDeclarationSyntax syntax)
        {
            return null;
        }

        var model = context.SemanticModel;
        var compilation = model.Compilation;
        var markerType = compilation.GetTypeByMetadataName(MarkerAttributeName);
        if (markerType is null)
        {
            return null;
        }

        if (!IsAssemblyEligibleForGeneration(compilation.Assembly, markerType))
        {
            return null;
        }

        if (model.GetDeclaredSymbol(syntax, cancel) is not INamedTypeSymbol symbol)
        {
            return null;
        }

        var testClassType = compilation.GetTypeByMetadataName(TestClassAttributeName);
        if (testClassType is null)
        {
            return null;
        }

        if (HasTestClassAttribute(symbol, testClassType))
        {
            return null;
        }

        // Only run generator for classes when the parent won't _also_ have generation.
        // Otherwise the generator will hide the base member.
        if (HasParentWithTestClassAttribute(symbol, testClassType) ||
            HasParentWithMarkerAttribute(symbol, markerType))
        {
            return null;
        }

        return Parser.Parse(symbol, syntax, cancel);
    }

    static bool HasTestClassAttribute(INamedTypeSymbol symbol, INamedTypeSymbol testClassType) =>
        !symbol.HasAttributeOfType(testClassType, includeDerived: true);

    static bool IsSyntaxEligibleForGeneration(SyntaxNode node, Cancel _) =>
        node is ClassDeclarationSyntax;

    static bool IsAssemblyEligibleForGeneration(IAssemblySymbol assembly, INamedTypeSymbol markerType) =>
        assembly.HasAttributeOfType(markerType, includeDerived: false);

    static bool HasParentWithMarkerAttribute(INamedTypeSymbol symbol, INamedTypeSymbol markerType) =>
        symbol
        .GetBaseTypes()
        .Any(_ => _.HasAttributeOfType(markerType, includeDerived: false));

    static bool HasParentWithTestClassAttribute(INamedTypeSymbol symbol, INamedTypeSymbol testClassType) =>
        symbol
        .GetBaseTypes()
        .Any(_ => _.HasAttributeOfType(testClassType, includeDerived: true));

    static void Execute(SourceProductionContext context, ImmutableArray<ClassToGenerate> toGenerate)
    {
        if (toGenerate.IsDefaultOrEmpty)
        {
            return;
        }

        var classes = toGenerate.Distinct();

        var emitter = new Emitter();
        var sourceCode = emitter.GenerateExtensionClasses(classes, context.CancellationToken);
        context.AddSource("UsesVerify.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
    }
}