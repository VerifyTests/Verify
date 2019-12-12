using System.Threading.Tasks;

partial class Verifier
{
    public Task Verify(string target)
    {
        Guard.AgainstNull(target, nameof(target));
        return Verify(target, textExtension);
    }
}