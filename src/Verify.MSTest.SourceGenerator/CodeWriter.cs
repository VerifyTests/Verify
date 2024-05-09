namespace Verify.MSTest.SourceGenerator;

static class CodeWriter
{
    // TODO: Add generated code decorations
    //private const string GeneratedTypeSummary =
    //            "<summary> " +
    //            "This API supports the logging infrastructure and is not intended to be used directly from your code. " +
    //            "It is subject to change in the future. " +
    //            "</summary>";
    //private static readonly string s_generatedCodeAttribute =
    //    $"global::System.CodeDom.Compiler.GeneratedCodeAttribute(" +
    //    $"\"{typeof(CodeWriter).Assembly.GetName().Name}\", " +
    //    $"\"{typeof(CodeWriter).Assembly.GetName().Version}\")";
    //private const string EditorBrowsableAttribute =
    //    "global::System.ComponentModel.EditorBrowsableAttribute(" +
    //    "global::System.ComponentModel.EditorBrowsableState.Never)";

    private static void WriteNamespace(IndentedStringBuilder sb, ClassToGenerate classToGenerate)
    {
        if (!string.IsNullOrEmpty(classToGenerate.Namespace)) // TODO: Consider making nullable?
        {
            sb.AppendLine($"namespace {classToGenerate.Namespace}");
            sb.AppendLine("{");

            sb.IncreaseIndent();
        }

        WriteParentTypes(sb, classToGenerate);

        if (!string.IsNullOrEmpty(classToGenerate.Namespace)) // TODO: Consider making nullable?
        {
            sb.DecreaseIndent();
            sb.AppendLine("}");
        };
    }

    private static void WriteParentTypes(IndentedStringBuilder sb, ClassToGenerate classToGenerate)
    {
        var parentClass = classToGenerate.ParentClass;
        var depth = 1;
        while (parentClass is not null)
        {
            sb.AppendLine($"partial {parentClass.Keyword} {parentClass.Name}{parentClass.Constraints}");
            sb.AppendLine("{");

            sb.IncreaseIndent();
            depth += 1;
            parentClass = parentClass.Child;
        }

        WriteClass(sb, classToGenerate);

        while (depth < 0)
        {
            sb.DecreaseIndent();
            sb.AppendLine("}");

            depth -= 1;
        }
    }

    private static void WriteClass(IndentedStringBuilder sb, ClassToGenerate classToGenerate)
    {
        var genericConstraints = string.Empty;

        if (classToGenerate.TypeParameters.Count > 0)
        {
            genericConstraints = $"<{string.Join(", ", classToGenerate.TypeParameters)}>";
        }

        sb.AppendLine($"partial class {classToGenerate.ClassName}{genericConstraints}");
        sb.AppendLine("{");
        sb.AppendLine("    public TestContext TestContext");
        sb.AppendLine("    {");
        sb.AppendLine("        get => CurrentTestContext.Value!;");
        sb.AppendLine("        set => CurrentTestContext.Value = value;");
        sb.AppendLine("    }");
        sb.AppendLine("}");
    }

    public static string GenerateExtensionClass(ClassToGenerate classToGenerate)
    {
        var sb = new IndentedStringBuilder();

        WriteNamespace(sb, classToGenerate);

        return sb.ToString();
    }
}
