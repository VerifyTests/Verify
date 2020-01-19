class ProcessCommand
{
    public string Command { get; }
    public uint Process { get; }

    public ProcessCommand(string command, in uint process)
    {
        Command = command;
        Process = process;
    }
}