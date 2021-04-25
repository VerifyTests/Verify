using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

static class HttpContentExtensions
{
    // From https://github.com/samuelneff/MimeTypeMap/blob/master/MimeTypeMap.cs
    static Dictionary<string, string> mappings = new(StringComparer.OrdinalIgnoreCase)
    {
        //extra
        {"application/graphql", "gql"},
        {"application/json", "json"},

        {"application/fsharp-script", "fsx"},
        {"application/msaccess", "adp"},
        {"application/msword", "doc"},
        {"application/octet-stream", "bin"},
        {"application/onenote", "one"},
        {"application/postscript", "eps"},
        {"application/step", "step"},
        {"application/vnd.ms-excel", "xls"},
        {"application/vnd.ms-powerpoint", "ppt"},
        {"application/vnd.ms-works", "wks"},
        {"application/vnd.visio", "vsd"},
        {"application/x-director", "dir"},
        {"application/x-msdos-program", "exe"},
        {"application/x-shockwave-flash", "swf"},
        {"application/x-x509-ca-cert", "cer"},
        {"application/x-zip-compressed", "zip"},
        {"application/xhtml+xml", "xhtml"},
        {"application/xml", "xml"},
        {"audio/aac", "aac"},
        {"audio/aiff", "aiff"},
        {"audio/basic", "snd"},
        {"audio/mid", "midi"},
        {"audio/mp4", "m4a"},
        {"audio/wav", "wav"},
        {"audio/x-m4a", "m4a"},
        {"audio/x-mpegurl", "m3u"},
        {"audio/x-pn-realaudio", "ra"},
        {"audio/x-smd", "smd"},
        {"image/bmp", "bmp"},
        {"image/jpeg", "jpg"},
        {"image/pict", "pic"},
        {"image/png", "png"},
        {"image/x-png", "png"},
        {"image/tiff", "tiff"},
        {"image/x-macpaint", "mac"},
        {"image/x-quicktime", "qti"},
        {"message/rfc822", "eml"},
        {"text/calendar", "ics"},
        {"text/html", "html"},
        {"text/plain", "txt"},
        {"text/scriptlet", "wsc"},
        {"text/xml", "xml"},
        {"video/3gpp", "3gp"},
        {"video/3gpp2", "3gp2"},
        {"video/mp4", "mp4"},
        {"video/mpeg", "mpg"},
        {"video/quicktime", "mov"},
        {"video/vnd.dlna.mpeg-tts", "m2t"},
        {"video/x-dv", "dv"},
        {"video/x-la-asf", "lsf"},
        {"video/x-ms-asf", "asf"},
        {"x-world/x-vrml", "xof"}
    };

    public static string ReadAsString(this HttpContent content)
    {
        return content.ReadAsStringAsync().GetAwaiter().GetResult();
    }

#if(!NET5_0)
    public static System.IO.Stream ReadAsStream(this HttpContent content)
    {
        return content.ReadAsStreamAsync().GetAwaiter().GetResult();
    }
#endif

    public static bool TryGetExtension(this HttpContent content, [NotNullWhen(true)] out string? extension)
    {
        var mediaType = content.Headers.ContentType?.MediaType;
        if (mediaType == null)
        {
            extension = null;
            return false;
        }

        return mappings.TryGetValue(mediaType, out extension);
    }

    public static bool IsText(this HttpContent content, [NotNullWhen(true)] out string? subType)
    {
        var mediaType = content.Headers.ContentType?.MediaType;
        if (mediaType == null)
        {
            subType = null;
            return false;
        }

        var split = mediaType.Split('/');
        subType = split[1];
        if (mappings.TryGetValue(mediaType, out var extension))
        {
            return EmptyFiles.Extensions.IsText(extension);
        }

        return false;
    }
}