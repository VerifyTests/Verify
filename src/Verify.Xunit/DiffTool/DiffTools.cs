using System.Collections.Generic;

namespace VerifyXunit
{
    public static partial class DiffTools
    {
        public static DiffTool BeyondCompare = new DiffTool(
            name: "BeyondCompare",
            binaryExtensions: new List<string>
            {
                "mp3",
                "xls",
                "xlsm",
                "xlsx",
                "doc",
                "docm",
                "docx",
                "dot",
                "dotm",
                "dotx",
                "pdf",
                "bmp",
                "gif",
                "ico",
                "jpg",
                "png",
                "tif",
                "rtf"
            });
    }
}