using System;
using System.IO;
using System.Text;

namespace VerifyTests
{
    [Obsolete("Use Target")]
    public readonly struct ConversionStream
    {
    }

    public readonly struct Target
    {
        readonly string? stringData;
        readonly StringBuilder? stringBuilderData;
        readonly Stream? streamData;
        public string Extension { get; }

        public Stream StreamData
        {
            get
            {
                if (streamData == null)
                {
                    throw new("Use StringData or StringBuilderData.");
                }

                return streamData;
            }
        }

        public bool IsStream { get => streamData != null; }

        public string StringData
        {
            get
            {
                if (stringData == null)
                {
                    throw new("Use StreamData or StringBuilderData.");
                }

                return stringData;
            }
        }

        public bool IsString { get => stringData != null; }

        public StringBuilder StringBuilderData
        {
            get
            {
                if (stringBuilderData == null)
                {
                    throw new("Use StreamData or StringData.");
                }

                return stringBuilderData;
            }
        }

        public bool IsStringBuilder { get => stringBuilderData != null; }

        public Target(string extension, Stream streamData)
        {
            Guard.AgainstBadExtension(extension, nameof(extension));
            Guard.AgainstNull(streamData, nameof(streamData));

            if (EmptyFiles.Extensions.IsText(extension))
            {
                throw new("Dont pass a stream for text. Instead use `Target(string extension, string stringData)` or `Target(string extension, StringBuilder stringBuilderData)`.");
            }

            Extension = extension;
            this.streamData = streamData;
            stringData = null;
            stringBuilderData = null;
        }

        public Target(string extension, StringBuilder stringBuilderData)
        {
            Guard.AgainstBadExtension(extension, nameof(extension));
            Guard.AgainstNull(stringBuilderData, nameof(stringBuilderData));
            if (!EmptyFiles.Extensions.IsText(extension))
            {
                throw new("Dont pass a text for a binary extension. Instead use `Target(string extension, Stream streamData)`.");
            }

            Extension = extension;
            stringData = null;
            streamData = null;
            this.stringBuilderData = stringBuilderData;
        }

        public Target(string extension, string stringData)
        {
            Guard.AgainstBadExtension(extension, nameof(extension));
            Guard.AgainstNull(stringData, nameof(stringData));
            if (!EmptyFiles.Extensions.IsText(extension))
            {
                throw new("Dont pass a text for a binary extension. Instead use `Target(string extension, Stream streamData)`.");
            }

            Extension = extension;
            this.stringData = stringData;
            streamData = null;
            stringBuilderData = null;
        }
    }
}