namespace Verify.MSTest.SourceGenerator;

static class CodeTemplates
{
    public static string GenerateExtensionClass(ClassToGenerate classToGenerate) =>
        $$"""
        namespace {{classToGenerate.Namespace}}
        {
            partial class {{classToGenerate.ClassName}}
            {
                public TestContext TestContext
                {
                    get => CurrentTestContext.Value!;
                    set => CurrentTestContext.Value = value;

                }
            }
        }
        """;
}
