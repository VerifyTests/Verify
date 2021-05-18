using System;
using System.Collections.Generic;
using System.IO;

static class FileToTypeFinder
{
    public static bool FindTypeFromFile(this List<Type> types, string file, out Type? type)
    {
        var withoutExtension = file[..file.LastIndexOf('.')];
        var withDots = withoutExtension
            .Replace(Path.DirectorySeparatorChar, '.')
            .Replace(Path.AltDirectorySeparatorChar, '.');
        foreach (var item in types)
        {
            if (withDots.EndsWith($".{item.FullName}"))
            {
                type = item;
                return true;
            }
        }

        foreach (var item in types)
        {
            if (withDots.EndsWith($".{item.Name}"))
            {
                type = item;
                return true;
            }
        }

        type = null;
        return false;
    }
}