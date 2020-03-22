using System.IO;
using System.Text;

static class StreamHelpers
{
    public static StreamWriter BuildLeaveOpenWriter(this Stream input)
    {
        return new StreamWriter(input, Encoding.UTF8, 1024, leaveOpen: true);
    }
}