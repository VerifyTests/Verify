using System;

namespace DiffEngine
{
    public class ResolvedDiffTool
    {
        public string Name { get; }
        public string ExePath { get; }
        public Func<string,string, string> BuildArguments { get; }
        public bool IsMdi { get; }
        public bool SupportsAutoRefresh { get; }

        public string BuildCommand(string path1, string path2)
        {
            return $"\"{ExePath}\" {BuildArguments(path1,path2)}";
        }

        public ResolvedDiffTool(
            string name,
            string exePath,
            Func<string, string, string> buildArguments,
            bool isMdi,
            bool supportsAutoRefresh)
        {
            Name = name;
            ExePath = exePath;
            BuildArguments = buildArguments;
            IsMdi = isMdi;
            SupportsAutoRefresh = supportsAutoRefresh;
        }
    }
}