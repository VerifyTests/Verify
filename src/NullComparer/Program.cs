using System.IO;
using NullComparer;

if (args.Length == 2)
{
    File.WriteAllText(args[1], Constants.KeyText);
}