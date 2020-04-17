using System;

static class NCrunch
{
    public static bool Enabled = Environment.GetEnvironmentVariable("NCrunch") == "1";
}