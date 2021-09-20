using System.Linq;

namespace VerifyTests
{
    /// <summary>
    /// Not for public use.
    /// </summary>
    public class FileNameBuilder
    {
        public string FilePathPrefix;

        public FileNameBuilder(
            MethodInfo method,
            Type type,
            string sourceFile,
            VerifySettings settings)
        {
            var (fileNamePrefix, directory) = FileNamePrefix(method, type, sourceFile, settings);

            var sourceFileDirectory = Path.GetDirectoryName(sourceFile)!;
            if (directory is null)
            {
                directory = sourceFileDirectory;
            }
            else
            {
                directory = Path.Combine(sourceFileDirectory, directory);
                Directory.CreateDirectory(directory);
            }

            FilePathPrefix = Path.Combine(directory, fileNamePrefix);
            PrefixUnique.CheckPrefixIsUnique(FilePathPrefix);

            var pattern = $"{fileNamePrefix}.*.*";
            var files = Directory.EnumerateFiles(directory, pattern).ToList();
            VerifiedFiles = MatchingFileFinder.Find(files, fileNamePrefix, ".verified").ToList();
            ReceivedFiles = MatchingFileFinder.Find(files, fileNamePrefix, ".received").ToList();
        }

        public static (string fileNamePrefix, string? directory) FileNamePrefix(MethodInfo method, Type type, string sourceFile, VerifySettings settings)
        {
            var pathInfo = VerifierSettings.GetPathInfo(sourceFile, type, method);

            var fileNamePrefix = GetFileNamePrefix(method, type, settings, pathInfo);
            var directory = settings.directory ?? pathInfo.Directory;
            return (fileNamePrefix, directory);
        }

        static string GetFileNamePrefix(MethodInfo method, Type type, VerifySettings settings, PathInfo pathInfo)
        {
            var uniquenessParts = PrefixUnique.GetUniquenessParts(settings.Namer);
            if (settings.fileName is not null)
            {
                return settings.fileName + uniquenessParts;
            }

            var typeName = settings.typeName ?? pathInfo.TypeName ?? GetTypeName(type);
            var methodName = settings.methodName ?? pathInfo.MethodName ?? method.Name;

            var parameterText = GetParameterText(method, settings);

            return $"{typeName}.{methodName}{parameterText}{uniquenessParts}";
        }

        static string GetParameterText(MethodInfo method, VerifySettings settings)
        {
            if (settings.parametersText is not null)
            {
                return $"_{settings.parametersText}";
            }

            if (settings.parameters is not null)
            {
                return $"_{ParameterBuilder.Concat(method, settings.parameters)}";
            }

            var methodParameters = method.GetParameters();
            if (methodParameters.IsEmpty())
            {
                return "";
            }

            var names = string.Join(", ", methodParameters.Select(x => x.Name));
            throw new($@"Method `{method.DeclaringType!.Name}.{method.Name}` requires parameters, but none have been defined. Add UseParameters. For example:

var settings = new VerifySettings();
settings.UseParameters({names});
await Verifier.Verify(target, settings);

or

await Verifier.Verify(target).UseParameters({names});
");
        }

        public List<string> VerifiedFiles { get; }

        public List<string> ReceivedFiles { get; }

        static string GetTypeName(Type type)
        {
            if (type.IsNested)
            {
                return $"{type.ReflectedType!.Name}.{type.Name}";
            }

            return type.Name;
        }
    }
}