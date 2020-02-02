
using System.Diagnostics;

namespace DiffEngine
{
    [DebuggerDisplay("{Command} | Process = {Process}")]
    public class ProcessCommand
    {
        public string Command { get; }
        public uint Process { get; }

        public ProcessCommand(string command, in uint process)
        {
            Guard.AgainstNullOrEmpty(command, nameof(command));
            Command = command;
            Process = process;
        }
    }
}