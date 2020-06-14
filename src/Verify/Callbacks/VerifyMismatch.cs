using System.Threading.Tasks;

namespace VerifyTesting
{
    public delegate Task VerifyMismatch(string receivedFile, string verifiedFile, string? message);
}