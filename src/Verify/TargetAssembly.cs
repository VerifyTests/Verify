namespace VerifyTests
{
    /// <summary>
    /// Not for public use.
    /// </summary>
    public static class TargetAssembly
    {
        static Assembly? assembly;
        public static string ProjectDirectory { get; private set; } = null!;
        public static string? SolutionDirectory { get; private set; }

        internal static void Assign(Assembly assembly)
        {
            if (TargetAssembly.assembly != null)
            {
                return;
            }
            Namer.UseAssembly(assembly);
            ProjectDirectory = AttributeReader.GetProjectDirectory(assembly);
            AttributeReader.TryGetSolutionDirectory(assembly, out var solutionDirectory);
            SolutionDirectory = solutionDirectory;
            ApplyScrubbers.UseAssembly(solutionDirectory, ProjectDirectory);
            TargetAssembly.assembly = assembly;
        }
    }
}