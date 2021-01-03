using System.Diagnostics;

namespace VerifyTests
{
    [DebuggerDisplay("Directory = {Directory} | TypeName = {TypeName} | MethodName = {MethodName}")]
    public readonly struct PathInfo
    {
        public string Directory { get; }
        public string TypeName { get; }
        public string MethodName { get; }

        public PathInfo(string directory, string typeName, string methodName)
        {
            Guard.AgainstNullOrEmpty(directory, nameof(directory));
            Guard.AgainstNullOrEmpty(typeName, nameof(typeName));
            Guard.AgainstNullOrEmpty(methodName, nameof(methodName));
            TypeName = typeName;
            MethodName = methodName;
            Directory = directory;
        }
    }
}