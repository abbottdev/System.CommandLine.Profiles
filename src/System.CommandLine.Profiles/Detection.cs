using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace System.CommandLine.Profiles
{
    public static class Detection
    {
        public static async Task<Profile> DetectCliAsync(bool currentProcess) {
            var shell = System.Environment.GetEnvironmentVariable("SHELL");

            return shell switch {
                "/bin/zsh" when currentProcess == false => Profile.Unix.Zsh,
                "/bin/bash" when currentProcess == false => Profile.Unix.Bash,
                _ when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => 
                    await DetermineMacOSShellAsync(shell, currentProcess),
                _ when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => 
                    await DetermineLinuxShellAsync(shell, currentProcess), 
                _ when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) =>
                    await DetermineWindowsShellAsync(),
                _ => throw new NotSupportedException("Unknown platform/shell")
            };
        }

        private static Task<Profile> DetermineWindowsShellAsync() => Task.FromResult(Profile.Windows.Cmd);

        private static async Task<Profile> DetermineMacOSShellAsync(string shellEnvVar, bool currentProcess)
        {
            if (currentProcess)
            {
                return await ParseUnixCallTreeForShellAsync(shellEnvVar);
            }
            else {
                throw new NotSupportedException($"Unknown default shell {shellEnvVar}");
            }
        }

        private static async Task<Profile> ParseUnixCallTreeForShellAsync(string shellEnvVar)
        {
            static (string pid, string ppid, string pgid, string comm) parsePs(string psOutput)
            {
                var lines = psOutput.Split(Environment.NewLine);
                if (lines != null && lines.Length > 1)
                {
                    var tabs = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    return (tabs[0].Trim(), tabs[1].Trim(), tabs[2].Trim(), tabs[3].Trim());
                }
                else
                    return default;
            }

            var processId = System.Environment.ProcessId;
            Profile shell;

            do
            {
                var (_, ppid, _, comm) = parsePs(await ProcessExtensions.RunAndCaptureOutputAsync("ps", $"-p {processId} -o pid,ppid,pgid,comm"));
                shell = comm switch
                {
                    "bash" => Profile.Unix.Bash,
                    "/bin/bash" => Profile.Unix.Bash,
                    "zsh" => Profile.Unix.Zsh,
                    "/bin/zsh" => Profile.Unix.Zsh,
                    _ when comm.Contains("pwsh") => Profile.CrossPlatform.Powershell,
                    _ => null
                };
                processId = int.Parse(ppid.Trim());

            } while (processId > 1 && shell == null);

            if (shell == null)
                throw new NotSupportedException($"Unknown shell {shellEnvVar}");

            return shell;
        }

        private static async Task<Profile> DetermineLinuxShellAsync(string shell, bool currentProcess)
        {
            if (currentProcess) {
                return await ParseUnixCallTreeForShellAsync(shell);
            } else {
                using var process = Process.Start("cat", "/proc/$$/comm");
                var output = await process.StandardOutput.ReadToEndAsync();

                return output switch
                {
                    "bash" => Profile.Unix.Bash,
                    "zsh" => Profile.Unix.Zsh,
                    "pwsh" => Profile.CrossPlatform.Powershell,
                    _ => await GetLinuxDefaultShellAsync()
                };
            }
        }
        
        private static async Task<Profile> GetLinuxDefaultShellAsync() {
            var distribution = await GetLinuxDistributionAsync();

            return distribution switch {
                "Ubuntu" => Profile.Unix.Bash,
                _ => throw new NotSupportedException($"Unable to determine the default shell for distribution: {distribution}")
            };
        }

        private static async Task<string> GetLinuxDistributionAsync() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ) {
                throw new Exception("Unable to determine the linux distribution when not running on Linux");
            }

            using(var process = Process.Start("cat", "/etc/*-release")) {
                var line = "";

                while ((line = await process.StandardOutput.ReadLineAsync()) != null)
                {
                     if (line.StartsWith("NAME=")) {
                         return line.Substring("NAME=\"".Length - 1, line.Length - 1);
                     }
                }
            }
            throw new NotSupportedException("Could not locate distribution information");
        }
    }
}
