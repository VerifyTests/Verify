using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

static class HttpContentExtensions
{
    public static bool IsText(this HttpContent content)
    {
        return IsText(content, out _);
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
        var type= split[0];
        subType= split[1];
        return type == "text" ||
               subType =="graphql" ||
               subType =="javascript" ||
               subType =="json" ||
               subType =="xml";
    }
}