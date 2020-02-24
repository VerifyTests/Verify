using DiffEngine;

namespace Verify
{
    public static partial class SharedVerifySettings
    {
        static SharedVerifySettings()
        {
            DiffRunner.MaxInstancesToLaunch(5);
        }
    }
}