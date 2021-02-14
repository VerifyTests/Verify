using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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

            instanceScrubbers = new(settings.instanceScrubbers);
            extension = settings.extension;
            clipboardEnabled = settings.clipboardEnabled;
            diffEnabled = settings.diffEnabled;
            methodName = settings.methodName;
            typeName = settings.typeName;
            directory = settings.directory;
            autoVerify = settings.autoVerify;
            serialization = settings.serialization;
            stringComparer = settings.stringComparer;
            streamComparer = settings.streamComparer;
            parameters = settings.parameters;
            fileName = settings.fileName;
            Namer = new(settings.Namer);
            foreach (var pair in settings.Context)
            {
                if (pair.Value is ICloneable cloneable)
                {
                    Context.Add(pair.Key, cloneable.Clone());
                }
                else
                {
                    Context.Add(pair.Key, pair.Value);
                }
            }
        }

        /// <summary>
        /// Allows extensions to Verify to pass config via <see cref="VerifySettings"/>.
        /// </summary>
        public Dictionary<string, object> Context { get; } = new();

        public VerifySettings()
        {
        }

        internal string? extension;

        /// <summary>
        /// Use a custom file extension for the test results.
        /// Where the file format is `{Directory}/{TestClassName}.{TestMethodName}.{Parameters}.{UniqueFor1}.{UniqueFor2}.{UniqueForX}.verified.{extension}`.
        /// </summary>
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

        internal string ExtensionOrTxt(string defaultValue = "txt")
        {
            if (extension == null)
            {
                return defaultValue;
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

        internal bool autoVerify;

        /// <summary>
        /// Automatically accept the results of the current tes.
        /// </summary>
        public void AutoVerify()
        {
            autoVerify = true;
        }
    }
}