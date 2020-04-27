using System.Threading.Tasks;

namespace Verify
{
    public delegate Task VerifyMismatch(string receivedFile, string verifiedFile, string? message);
}