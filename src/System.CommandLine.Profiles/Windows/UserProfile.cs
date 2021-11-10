using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.CommandLine.Profiles.Windows {
    using Environment = System.Environment;

    internal class UserProfile : IProfileSource
    {
        public UserProfile() {
        }
        

        public Task<T> AppendVariableAsync<T>(string variable, T value, Profile profile, OSPlatform platform, string delimiter = ";", CancellationToken token = default)
        {
            Environment.SetEnvironmentVariable(variable, $"{value}", EnvironmentVariableTarget.User);
            return Task.FromResult(value);
        }

        public Task<T> SetVariableAsync<T>(string variable, T value, Profile profile, OSPlatform platform, CancellationToken token = default)
        {
            Environment.SetEnvironmentVariable(variable, $"{value}", EnvironmentVariableTarget.User);
            return Task.FromResult(value);
        }

        public bool SupportsProfile(Profile profile, OSPlatform platform) => platform == OSPlatform.Windows;
    }
}