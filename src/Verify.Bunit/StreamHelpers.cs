using System.IO;
using System.Text;

static class StreamHelpers
{
    static Encoding utf8NoBOM = new UTF8Encoding(false, true);

    public static StreamWriter BuildLeaveOpenWriter(this Stream input)
    {
        return new StreamWriter(input, utf8NoBOM, 1024, leaveOpen: true);
    }
}