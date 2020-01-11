using System;

static class NCrunch
{
    public static bool Enabled()
    {
        return Environment.GetEnvironmentVariable("NCrunch") == "1";
    }
}