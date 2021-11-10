using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace System.CommandLine.Profiles.Unix {
    public abstract class UnixExportSource : IProfileFileSource
    {
        private const string RegexExpression = @"^EXPORT\s*([^=\s])\s*=(.*)\s+$";

        public abstract string ProfileFilePath(Profile profile, OSPlatform platform);

        public abstract bool SupportsProfile(Profile profile, OSPlatform platform);

        public async Task<T> SetVariableAsync<T>(string variable, T value, Profile profile, OSPlatform platform, CancellationToken token = default) {
            var lines = (await File.ReadAllLinesAsync(ProfileFilePath(profile, platform), token)).ToList();
            var targets = ReadProfile(lines.ToArray());

            if (targets.Count > 1) {
                //Comment out additional lines 
                targets.ForEach((tuple) => {
                    tuple.value = "# " + tuple.value + " - Hidden by System.CommandLine.Profiles";
                });
            }

            lines.Add($"EXPORT {variable}={value}");

            foreach (var target in targets)
            {
                lines[target.index] = target.value;
            }

            // await File.WriteAllLinesAsync(ProfileFilePath, lines, Text.Encoding.UTF8, token);

            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }

            return value;
        }

        private static List<(int index, string line, string value)> ReadProfile(string[] lines) {
            var targets = new List<(int index, string line, string value)>();
            var regex = new Regex(RegexExpression, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line.StartsWith("EXPORT ")) {
                    var match = regex.Match(line);
                    if (match.Success) {
                        targets.Add((i, line, match.Groups[2].Value));
                    }
                }
            }

            return targets;
        }

        public async Task<T> AppendVariableAsync<T>(string variable, T value, Profile profile, OSPlatform platform, string delimiter = ";", CancellationToken token = default) {
            var lines = await File.ReadAllLinesAsync(ProfileFilePath(profile, platform), token);
            var targets = ReadProfile(lines);

            if (targets.Count > 1) {
                //Append a line to append the value
                lines = lines.Append($"EXPORT {variable}=${variable}{delimiter}{value}").ToArray();
            } else {
                lines = lines.Append($"EXPORT {variable}={value}").ToArray();
            }

            foreach (var target in targets)
            {
                lines[target.index] = target.value;
            }

            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
            //await File.WriteAllLinesAsync(ProfileFilePath, lines, Text.Encoding.UTF8, token);

            return value;
        }

    }
}