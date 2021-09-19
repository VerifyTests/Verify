using System.Linq;

namespace VerifyTests
{
    /// <summary>
    /// Not for public use.
    /// </summary>
    public class FileNameBuilder
    {
        string filePathPrefix;

        public FileNameBuilder(
            MethodInfo method,
            Type type,
            string projectDirectory,
            string sourceFile,
            VerifySettings settings)
        {
            var namer = settings.Namer;

            var pathInfo = VerifierSettings.GetPathInfo(sourceFile, projectDirectory, type, method);

            var directory = settings.directory ?? pathInfo.Directory;

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

            var fileNamePrefix = GetFileNamePrefix(method, type, settings, pathInfo, namer);
            filePathPrefix = Path.Combine(directory, fileNamePrefix);
            PrefixUnique.CheckPrefixIsUnique(filePathPrefix);

            var pattern = $"{fileNamePrefix}.*.*";
            var files = Directory.EnumerateFiles(directory, pattern).ToList();
            VerifiedFiles = MatchingFileFinder.Find(files, fileNamePrefix, ".verified").ToList();
            ReceivedFiles = MatchingFileFinder.Find(files, fileNamePrefix, ".received").ToList();
        }

        static string GetFileNamePrefix(MethodInfo method, Type type, VerifySettings settings, PathInfo pathInfo, Namer namer)
        {
            var uniquenessParts = PrefixUnique.GetUniquenessParts(namer, type.Assembly);
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
            if (!methodParameters.Any())
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

        public FilePair GetFileNames(string extension)
        {
            return new(extension, filePathPrefix);
        }

        public FilePair GetFileNames(string extension, int index)
        {
            return new(extension, $"{filePathPrefix}.{index:D2}");
        }
    }
}