using System.Threading.Tasks;

namespace VerifyTests
{
    public delegate Task VerifyMismatch(string receivedFile, string verifiedFile, string? message);
}