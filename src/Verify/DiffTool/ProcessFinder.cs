using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;

static class ProcessFinder
{
    public static IEnumerable<string> Find()
    {
        var wmiQuery = @"
select CommandLine
from Win32_Process
where (NOT (CommandLine Is Null)) and
      CommandLine like '%"" ""%'";
        var searcher = new ManagementObjectSearcher(wmiQuery);
        var retObjectCollection = searcher.Get();
        foreach (var retObject in retObjectCollection)
        {
            var arg0 = (string) retObject["CommandLine"];
            yield return arg0;
        }
    }
}