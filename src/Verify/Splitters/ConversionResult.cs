using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace VerifyTests
{
    public readonly struct ConversionResult
    {
        public object? Info { get; }

        public IEnumerable<Target> Targets { get; }

        public Func<Task>? Cleanup { get; }

        public ConversionResult(object? info, IEnumerable<Target> targets, Func<Task>? cleanup = null)
        {
            Guard.AgainstNull(targets, nameof(targets));
            Info = info;
            Targets = targets;
            Cleanup = cleanup;
        }

        public ConversionResult(object? info, string extension, Stream stream, Func<Task>? cleanup = null)
        {
            Guard.AgainstNull(stream, nameof(stream));
            Guard.AgainstNullOrEmpty(extension, nameof(extension));
            Info = info;
            Cleanup = cleanup;
            Targets = new List<Target>
            {
                new(extension, stream)
            };
        }

        public ConversionResult(object? info, string extension, string data, Func<Task>? cleanup = null)
        {
            Guard.AgainstNull(data, nameof(data));
            Guard.AgainstNullOrEmpty(extension, nameof(extension));
            Info = info;
            Cleanup = cleanup;
            Targets = new List<Target>
            {
                new(extension, data)
            };
        }

        public ConversionResult(object? info, string extension, StringBuilder data, Func<Task>? cleanup = null)
        {
            Guard.AgainstNull(data, nameof(data));
            Guard.AgainstNullOrEmpty(extension, nameof(extension));
            Info = info;
            Cleanup = cleanup;
            Targets = new List<Target>
            {
                new(extension, data)
            };
        }
    }
}