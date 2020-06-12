using System;
using System.Collections.Generic;
using System.Reflection;
using Verify;

static class TypeCache
{
    static List<Type> types = null!;

    public static MethodInfo GetInfo(string file, string method)
    {
        if (types == null)
        {
            if (SharedVerifySettings.assembly == null)
            {
                throw new Exception("Call `SharedVerifySettings.SetTestAssembly(Assembly.GetExecutingAssembly());` at assembly startup. Or, alternatively, a `[InjectInfo]` to the type.");
            }

            types = SharedVerifySettings.assembly.InstanceTypes();
        }

        if (types.FindTypeFromFile(file, out var type))
        {
            return type!.GetPublicMethod(method);
        }

        throw new Exception($"Unable to find type for file `{file}`. There are some known scenarios where the types cannot be derived (for example partial or nested classes). In these case add a `[InjectInfo]` to the type.");
    }
}