using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace System.CommandLine.Profiles.Unix {
    public class BashProfileSource : UnixExportSource
    {
        public override string ProfileFilePath(Profile profile, OSPlatform platform) 
            => Path.Combine(System.Environment.GetEnvironmentVariable("HOME"), "./.bash_profile");

        public override bool SupportsProfile(Profile profile, OSPlatform platform) => 
            new OSPlatform[] { OSPlatform.Linux, OSPlatform.OSX }.Contains(platform) 
                && profile == Profiles.Profile.Unix.Bash;
    }
}