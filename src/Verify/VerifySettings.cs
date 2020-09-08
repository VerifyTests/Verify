using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace VerifyTests
{
    public partial class VerifySettings
    {
        public VerifySettings(VerifySettings? settings)
        {
            if (settings == null)
            {
                return;
            }

            instanceScrubbers = new List<Action<StringBuilder>>(settings.instanceScrubbers);
            extension = settings.extension;
            SourceFile = settings.SourceFile;
            clipboardEnabled = settings.clipboardEnabled;
            diffEnabled = settings.diffEnabled;
            autoVerify = settings.autoVerify;
            serialization = settings.serialization;
            newLineEscapingDisabled = settings.newLineEscapingDisabled;
            handleOnFirstVerify = settings.handleOnFirstVerify;
            handleOnVerifyMismatch = settings.handleOnVerifyMismatch;
            Namer = new Namer(settings.Namer);
            foreach (var pair in settings.Data)
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

        /// <summary>
        /// Allows extensions to Verify to pass config via <see cref="VerifySettings"/>.
        /// </summary>
        [ObsoleteEx(
            ReplacementTypeOrMember = "VerifySettings.Context",
            RemoveInVersion = "7.0")]
        public IDictionary<string, object> Data { get => Context; }

        /// <summary>
        /// Allows extensions to Verify to pass config via <see cref="VerifySettings"/>.
        /// </summary>
        public IDictionary<string, object> Context { get; } = new Dictionary<string, object>();

        public string SourceFile { get; internal set; } = null!;

        public VerifySettings()
        {
        }

        internal string? extension;

        public void UseExtension(string extension)
        {
            Guard.AgainstBadExtension(extension, nameof(extension));
            this.extension = extension;
        }

        public bool TryGetExtension([NotNullWhen(true)] out string? extension)
        {
            if (this.extension == null)
            {
                extension = null;
                return false;
            }
            extension = this.extension;
            return true;
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