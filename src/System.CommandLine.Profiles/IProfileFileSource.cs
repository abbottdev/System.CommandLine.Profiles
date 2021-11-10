
using System.Runtime.InteropServices;

namespace System.CommandLine.Profiles {

    public interface IProfileFileSource : IProfileSource {
        string ProfileFilePath(Profile profile, OSPlatform platform);
    }
}