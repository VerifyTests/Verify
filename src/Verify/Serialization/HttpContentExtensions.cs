using System.Net.Http;

static class HttpContentExtensions
{
    public static bool IsText(this HttpContent content)
    {
        var type = content.Headers.ContentType?.MediaType;
        if (type == null)
        {
            return false;
        }

        return type.StartsWith("text") ||
               type.EndsWith("graphql") ||
               type.EndsWith("javascript") ||
               type.EndsWith("json") ||
               type.EndsWith("xml");
    }
}