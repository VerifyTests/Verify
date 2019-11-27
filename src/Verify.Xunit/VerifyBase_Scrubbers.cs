using System;
using System.Collections.Generic;
using System.IO;

namespace VerifyXunit
{
    public partial class VerifyBase
    {
        List<Func<string, string>> instanceScrubbers = new List<Func<string, string>>();

        public void ScrubMachineName()
        {
            AddScrubber(Scrubbers.ScrubMachineName);
        }

        public void AddScrubber(Func<string, string> scrubber)
        {
            Guard.AgainstNull(scrubber, nameof(scrubber));

            instanceScrubbers.Insert(0, scrubber);
        }

        string ApplyScrubbers(string target)
        {
            var baseDirectory = CleanPath(AppDomain.CurrentDomain.BaseDirectory);
            target = target.Replace(baseDirectory, "CurrentDirectory");
            var currentDirectory = CleanPath(Environment.CurrentDirectory);
            target = target.Replace(currentDirectory, "CurrentDirectory");
            var codeBase = CleanPath(CodeBaseLocation.CurrentDirectory);
            target = target.Replace(codeBase, "CurrentDirectory");

            foreach (var scrubber in instanceScrubbers)
            {
                target = scrubber(target);
            }

            foreach (var scrubber in Global.GlobalScrubbers)
            {
                target = scrubber(target);
            }

            return target;
        }

        string CleanPath(string directory)
        {
            return directory.TrimEnd('/', '\\');
        }
    }
}