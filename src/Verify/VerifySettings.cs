using System;
using System.Collections.Generic;

namespace Verify
{
    public partial class VerifySettings
    {
        internal static VerifySettings Default = new VerifySettings();

        public VerifySettings(VerifySettings? settingsToClone)
        {
            if (settingsToClone == null)
            {
                return;
            }

            instanceScrubbers = new List<Func<string, string>>(settingsToClone.instanceScrubbers);
            extension = settingsToClone.extension;
            clipboardEnabled = settingsToClone.clipboardEnabled;
            diffEnabled = settingsToClone.diffEnabled;
            autoVerify = settingsToClone.autoVerify;
            serialization = settingsToClone.serialization;
            newLineEscapingDisabled = settingsToClone.newLineEscapingDisabled;
            Namer = new Namer(settingsToClone.Namer);
            foreach (var pair in settingsToClone.Data)
            {
                if (pair.Value is ICloneable cloneable)
                {
                    Data.Add(pair.Key, cloneable.Clone());
                }
                else
                {
                    Data.Add(pair.Key, pair.Value);
                }
            }
        }

        public IDictionary<string, object> Data { get; } = new Dictionary<string, object>();

        public VerifySettings()
        {
        }

        internal string? extension;

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

        internal bool HasExtension()
        {
            return extension != null;
        }

        internal string ExtensionOrBin()
        {
            if (extension == null)
            {
                return "bin";
            }

            return extension;
        }

        internal bool autoVerify;

        public void AutoVerify()
        {
            autoVerify = true;
        }
    }
}