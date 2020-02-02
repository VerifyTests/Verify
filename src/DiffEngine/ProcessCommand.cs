
using System.Diagnostics;

namespace DiffEngine
{
    [DebuggerDisplay("{Command} | Process = {Process}")]
    public class ProcessCommand
    {
        /// <summary>
        /// The command line used to launch the process.
        /// </summary>
        public string Command { get; }

        /// <summary>
        /// The process Id.
        /// </summary>
        public uint Process { get; }

        public ProcessCommand(string command, in uint process)
        {
            Guard.AgainstNullOrEmpty(command, nameof(command));
            Command = command;
            Process = process;
        }
    }
}