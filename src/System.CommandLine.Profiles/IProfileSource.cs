using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;

namespace System.CommandLine.Profiles {

    public interface IProfileSource {
        bool SupportsProfile(Profile profile, OSPlatform platform);
        Task<T> SetVariableAsync<T>(string variable, T value, Profile profile, OSPlatform platform, CancellationToken token = default);
        Task<T> AppendVariableAsync<T>(string variable, T value, Profile profile, OSPlatform platform, string delimiter = ";", CancellationToken token = default);
    }
}