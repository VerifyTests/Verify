using System;
using System.Diagnostics;

static class EnvironmentEx
{
    public static string? GetEnvironmentVariable(string name)
    {
        var variable = Environment.GetEnvironmentVariable(name);
        if (variable != null)
        {
            return variable;
        }

        var replace = name.Replace('_', '.');
        variable = Environment.GetEnvironmentVariable(replace);
        if (variable != null)
        {
            Trace.WriteLine($"Found environment variable '{replace}'. Should use '{name}' instead.");
            return variable;
        }

        return variable;
    }
}