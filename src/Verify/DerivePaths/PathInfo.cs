using System.Diagnostics;

namespace VerifyTests
{
    [DebuggerDisplay("Directory = {Directory} | FilePrefix = {FilePrefix}")]
    public readonly struct PathInfo
    {
        public string Directory { get; }
        public string FilePrefix { get; }

        public PathInfo(string directory, string filePrefix)
        {
            Guard.AgainstNullOrEmpty(directory, nameof(directory));
            Guard.AgainstNullOrEmpty(filePrefix, nameof(filePrefix));
            FilePrefix = filePrefix;
            Directory = directory;
        }
    }
}