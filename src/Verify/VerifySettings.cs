﻿using System;
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
            clipboardEnabled = settings.clipboardEnabled;
            diffEnabled = settings.diffEnabled;
            autoVerify = settings.autoVerify;
            serialization = settings.serialization;
            handleOnFirstVerify = settings.handleOnFirstVerify;
            handleOnVerifyMismatch = settings.handleOnVerifyMismatch;
            comparer = settings.comparer;
            Namer = new Namer(settings.Namer);
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
        public Dictionary<string, object> Context { get; } = new Dictionary<string, object>();

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