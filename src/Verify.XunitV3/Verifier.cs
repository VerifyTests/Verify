#pragma warning disable VerifySetParameters
namespace VerifyXunit;

public static partial class Verifier
{
    static async Task AddFile(string path) =>
        TestContext.Current.AddAttachment(
            AttachmentName(path),
            await File.ReadAllBytesAsync(path));

    // xunit v3 uses the attachment name as the on-disk filename (via MediaTypeUtility.GetSanitizedFileNameWithExtension),
    // sanitizing only chars from Path.GetInvalidFileNameChars — which is OS-dependent. On Linux ':' is valid there,
    // but GitHub Actions upload-artifact rejects it, breaking CI snapshot upload. The full path is preserved (with
    // separators and the drive colon replaced) so nested snapshots that share a filename still produce unique keys,
    // since xunit throws on duplicate attachment names. See https://github.com/VerifyTests/Verify/issues/1721
    static string AttachmentName(string path)
    {
        var relative = path;
        var solutionDir = VerifierSettings.SolutionDir;
        if (solutionDir != null && path.StartsWith(solutionDir, StringComparison.OrdinalIgnoreCase))
        {
            relative = path[solutionDir.Length..].TrimStart('/', '\\');
        }

        var builder = new StringBuilder(relative.Length);
        foreach (var c in relative)
        {
            if (c is '/' or '\\' or ':')
            {
                builder.Append('_');
            }
            else
            {
                builder.Append(c);
            }
        }

        return builder.ToString();
    }

    [ModuleInitializer]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void AddAttachmentEvents() =>
        VerifierSettings.AddTestAttachment(AddFile);

    public static InnerVerifier BuildVerifier(VerifySettings settings, string sourceFile, bool useUniqueDirectory = false)
    {
        var context = TestContext.Current;
        var testMethod = context.GetTestMethod();

        var method = testMethod.Method;

        if (useUniqueDirectory)
        {
            settings.UseUniqueDirectory();
        }

        var parameterNames = method.ParameterNames();

        SetParametersFromContext(settings, context, parameterNames);

        var type = method.ReflectedType!;
        VerifierSettings.AssignTargetAssembly(type.Assembly);

        var pathInfo = GetPathInfo(sourceFile, type, method);
        return new(
            sourceFile,
            settings,
            type.NameWithParent(),
            method.Name,
            parameterNames,
            pathInfo);
    }

    static void SetParametersFromContext(VerifySettings settings, ITestContext context, IReadOnlyList<string>? parameterNames)
    {
        if (settings.HasParameters)
        {
            return;
        }

        if (context.TestCase is XunitTestCase { TestMethodArguments.Length: > 0 } testCase &&
            testCase.TestMethodArguments.Length == parameterNames?.Count)
        {
            settings.SetParameters(testCase.TestMethodArguments);
            return;
        }

        //For some reason when using `dotnet run` then TestCase is not a XunitTestCase
        if (context.Test is XunitTest { TestMethodArguments.Length: > 0 } test &&
            test.TestMethodArguments.Length == parameterNames?.Count)
        {
            settings.SetParameters(test.TestMethodArguments);
            return;
        }
    }

    [Pure]
    public static SettingsTask Verify(
        object? target,
        IEnumerable<Target> rawTargets,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            sourceFile,
            _ => _.Verify(target, rawTargets));

    [Pure]
    public static SettingsTask Verify(
        IEnumerable<Target> targets,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            sourceFile,
            _ => _.Verify(targets));

    [Pure]
    public static SettingsTask Verify(
        Target target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            sourceFile,
            _ => _.Verify(target));

    static SettingsTask Verify(
        VerifySettings? settings,
        string sourceFile,
        Func<InnerVerifier, Task<VerifyResult>> verify,
        bool useUniqueDirectory = false)
    {
        Guards.AgainstBadSourceFile(sourceFile);
        return new(
            settings,
            async settings =>
            {
                using var verifier = BuildVerifier(settings, sourceFile, useUniqueDirectory);
                return await verify(verifier);
            });
    }
}