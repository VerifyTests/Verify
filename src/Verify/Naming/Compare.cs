using System.IO;
using System.Threading.Tasks;

namespace Verify
{
    public delegate string ParameterToName<in T>(T parameter);
}