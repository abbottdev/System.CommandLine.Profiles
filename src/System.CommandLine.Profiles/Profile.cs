
namespace System.CommandLine.Profiles {
    public class Profile {
        public string Shell { get; init; }

        private Profile() {}

        public override bool Equals(object obj)
        {
            if (obj is Profile other) 
                return other.Shell.Equals(this.Shell);
            else
                return base.Equals(obj);
        }

        public override int GetHashCode() => this.Shell.GetHashCode();

        public static class CrossPlatform {
            public static Profile Powershell { get; } = new Profile() { 
                Shell = nameof(Powershell)};
        }

        public static class Unix {
            public static Profile Zsh {get;} = new Profile() { Shell = nameof(Zsh) };
            public static Profile Bash {get;} = new Profile() { Shell = nameof(Bash) };
        }

        public static class Windows {
            public static Profile Cmd {get;} = new Profile() { Shell = nameof(Cmd) };
        }
    }
}