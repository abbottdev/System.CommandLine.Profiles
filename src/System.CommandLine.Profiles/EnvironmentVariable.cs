
#nullable enable

namespace System.CommandLine.Profiles {
    using System.Linq;
    using System.Runtime.InteropServices;
    using SystemEnvironment = System.Environment;

    public static class Environment {
        public static bool HasShutdownStarted { get => SystemEnvironment.HasShutdownStarted; }
        public static bool Is64BitOperatingSystem { get => SystemEnvironment.Is64BitOperatingSystem; }
        public static string NewLine { get => SystemEnvironment.NewLine; }
        public static int ExitCode { get => SystemEnvironment.ExitCode; }
        public static int CurrentManagedThreadId { get => SystemEnvironment.CurrentManagedThreadId; }
        public static string ExpandEnvironmentVariables(string name) => SystemEnvironment.ExpandEnvironmentVariables(name);

        public static void SetEnvironmentVariable(string variable, string? value, EnvironmentVariableTarget target, Profile profile) {
            if (target == EnvironmentVariableTarget.Process) 
                SystemEnvironment.SetEnvironmentVariable(variable, value, target);

            var platform = profile switch {
                _ when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => OSPlatform.Windows,
                _ when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => OSPlatform.OSX,
                _ when RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD) => OSPlatform.FreeBSD,
                _ when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => OSPlatform.Linux,
                _ => throw new PlatformNotSupportedException()
            };

            var targetProfile = new IProfileSource[] {
                new Unix.ZshProfileSource(),
                new Unix.BashProfileSource()
            }.Where(p => p.SupportsProfile(profile, platform)).Single();

            if (targetProfile != null) {
                targetProfile.SetVariableAsync(variable, value, profile, platform).ConfigureAwait(false).GetAwaiter().GetResult();
            } else {
                throw new NotImplementedException($"Profile source for {profile.Shell} not implemented");
            }
        }
    }
}