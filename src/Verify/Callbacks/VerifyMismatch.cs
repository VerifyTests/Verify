using System.Threading.Tasks;

namespace VerifyTests
{
    public delegate Task VerifyMismatch(FilePair filePair, string? message);
}