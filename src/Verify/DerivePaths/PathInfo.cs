using System.Diagnostics;

namespace VerifyTests
{
    [DebuggerDisplay("Directory = {Directory} | TypeName = {TypeName} | MethodName = {MethodName}")]
    public readonly struct PathInfo
    {
        public string? Directory { get; }
        public string? TypeName { get; }
        public string? MethodName { get; }

        public PathInfo(
            string? directory = null,
            string? typeName = null,
            string? methodName = null)
        {
            Guard.AgainstEmpty(directory, nameof(directory));
            Guard.AgainstEmpty(typeName, nameof(typeName));
            Guard.AgainstEmpty(methodName, nameof(methodName));
            TypeName = typeName;
            MethodName = methodName;
            Directory = directory;
        }
    }
}