using Expecto;
using Expecto.CSharp;

namespace VerifyExpecto
{
    public static class ConfigExtensions
    {
        public static Impl.ExpectoConfig UseVerify(this Impl.ExpectoConfig config)
        {
            return config.AddPrinter(new CaptureName());
        }
    }
}