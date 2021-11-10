
using System.Diagnostics;
using System.Threading.Tasks;

namespace System.CommandLine.Profiles {
    public class ProcessExtensions {
        public static async Task<string> RunAndCaptureOutputAsync(string path, string arguments) {
            var start = new ProcessStartInfo(path, arguments)
            {
                RedirectStandardOutput = true
            };
            using var process = Process.Start(start);
            return await process.StandardOutput.ReadToEndAsync();
        }
    }
}