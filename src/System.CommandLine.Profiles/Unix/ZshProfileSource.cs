using System.IO;
using System.Linq; 
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace System.CommandLine.Profiles.Unix { 
    public class ZshProfileSource : UnixExportSource
    {
        public override string ProfileFilePath(Profile profile, OSPlatform platform) 
            => Path.Combine(System.Environment.GetEnvironmentVariable("HOME"), "./.zshrc");

        public override bool SupportsProfile(Profile profile, OSPlatform platform) => 
            new OSPlatform[] { OSPlatform.Linux, OSPlatform.OSX }.Contains(platform) 
                && profile == Profiles.Profile.Unix.Zsh;
    }
}