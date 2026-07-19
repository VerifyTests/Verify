static class EngineRunner
{
    public static Dictionary<string, object> EmptyContext { get; } = [];

    public static string Run(string input, params Scrubber[] scrubbers)
    {
        using var counter = Counter.Start();
        return Run(input, counter, scrubbers);
    }

    public static string Run(string input, Counter counter, params Scrubber[] scrubbers) =>
        ScrubEngine.Run(
            input,
            EngineScrubberSet.ForScrubbers([.. scrubbers]),
            counter,
            EmptyContext,
            applyDirectoryReplacements: false);

    public static string RunWithDirectoryReplacements(string input, params Scrubber[] scrubbers)
    {
        using var counter = Counter.Start();
        return ScrubEngine.Run(
            input,
            EngineScrubberSet.ForScrubbers([.. scrubbers]),
            counter,
            EmptyContext,
            applyDirectoryReplacements: true);
    }

    // Deterministic path replacements, matching the DisableScrubbersTests pattern.
    // Called from static ctors of test classes that exercise path replacement, so it
    // runs after the adapter has assigned the real target assembly paths.
    public static void UseFakeDirectories() =>
        DirectoryReplacements.UseAssembly("C:/Code/TheSolution", "C:/Code/TheSolution/TheProject");
}
