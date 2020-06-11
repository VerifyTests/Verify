using System;
using System.Linq;
using System.Reflection;

namespace Verify
{
    public static partial class SharedVerifySettings
    {
        internal static Assembly assembly = null!;
        internal static string? projectDirectory;

        public static void SetTestAssembly(Assembly assembly)
        {
            Guard.AgainstNull(assembly, nameof(assembly));

            projectDirectory = assembly
                .GetCustomAttributes<AssemblyMetadataAttribute>()
                .SingleOrDefault(x => x.Key == "Verify.ProjectDirectory")?.Value;
            SharedVerifySettings.assembly = assembly;
        }

        internal static void GetTestAssembly()
        {
            if (assembly == null)
            {
                throw new Exception("Call `SharedVerifySettings.SetTestAssembly(Assembly.GetExecutingAssembly());` at assembly startup. Or, alternatively, a `[InjectInfo]` to the type.");
            }
        }
    }
}