using System;
using System.Collections.Generic;

namespace Verify
{
    static class VerifySettingsEx
    {
        public static VerifySettings OrDefault(this VerifySettings? settings)
        {
            if (settings == null)
            {
                return VerifySettings.Default;
            }

            return settings;
        }
    }
    public partial class VerifySettings
    {
        internal static VerifySettings Default = new VerifySettings();
        internal string? extension;

        public VerifySettings(VerifySettings settingsToClone)
        {
            instanceScrubbers = new List<Func<string, string>>(settingsToClone.instanceScrubbers);
            extension = settingsToClone.extension;
            Namer = new Namer(settingsToClone.Namer);
        }

        public VerifySettings()
        {
        }

        public void UseExtension(string extension)
        {
            Guard.AgainstBadExtension(extension, nameof(extension));
            this.extension = extension;
        }

        internal string ExtensionOrTxt()
        {
            if (extension == null)
            {
                return "txt";
            }
            return extension;
        }
        internal string ExtensionOrBin()
        {
            if (extension == null)
            {
                return "bin";
            }
            return extension;
        }
    }
}