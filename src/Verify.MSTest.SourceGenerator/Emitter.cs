static class Emitter
{
    static readonly string AutoGenerationHeader = """
        //-----------------------------------------------------
        // This code was generated by a tool.
        //
        // Changes to this file may cause incorrect behavior
        // and will be lost when the code is regenerated.
        // <auto-generated />
        //-----------------------------------------------------
        """;

    static readonly string GeneratedCodeAttribute =
        $"[global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"{typeof(Emitter).Assembly.GetName().Name}\", \"{typeof(Emitter).Assembly.GetName().Version}\")]";

    static void WriteNamespace(IndentedStringBuilder builder, ClassToGenerate classToGenerate)
    {
        if (classToGenerate.Namespace is not null)
        {
            builder.Append("namespace ").AppendLine(classToGenerate.Namespace)
              .AppendLine("{")
              .IncreaseIndent();
        }

        WriteParentTypes(builder, classToGenerate);

        if (classToGenerate.Namespace is not null)
        {
            builder.DecreaseIndent()
              .AppendLine("}");
        };
    }

    static void WriteParentTypes(IndentedStringBuilder builder, ClassToGenerate classToGenerate)
    {
        foreach (var parentClass in classToGenerate.ParentClasses)
        {
            builder.Append("partial ").Append(parentClass.Keyword).Append(" ").AppendLine(parentClass.Name)
              .AppendLine("{");

            builder.IncreaseIndent();
        }

        WriteClass(builder, classToGenerate);

        foreach (var _ in classToGenerate.ParentClasses)
        {
            builder.DecreaseIndent()
              .AppendLine("}");
        }
    }

    static void WriteClass(IndentedStringBuilder builder, ClassToGenerate classToGenerate) =>
        builder.AppendLine(GeneratedCodeAttribute)
          .Append("partial class ").AppendLine(classToGenerate.ClassName)
          .AppendLine("{")
          .AppendLine("    public TestContext TestContext")
          .AppendLine("    {")
          .AppendLine("        get => Verifier.CurrentTestContext.Value!;")
          .AppendLine("        set => Verifier.CurrentTestContext.Value = value;")
          .AppendLine("    }")
          .AppendLine("}");

    public static string GenerateExtensionClasses(IReadOnlyCollection<ClassToGenerate> classesToGenerate)
    {
        var builder = new IndentedStringBuilder();
        builder.AppendLine(AutoGenerationHeader);

        foreach (var classToGenerate in classesToGenerate)
        {
            builder.AppendLine();
            WriteNamespace(builder, classToGenerate);
        }

        return builder.ToString();
    }
}
