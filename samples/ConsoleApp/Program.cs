using System;

namespace ConsoleApp
{
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.CommandLine.Profiles;
    using System.Threading.Tasks;

    class Program
    {
        static async Task<int> Main(string[] args)
        {
            
            var detect = new Command("detect") {
                new Option<bool>("--current", "Analyze the current process. (Default: false) to inspect the default shell for the OS")
            };
                        
            detect.Handler = CommandHandler.Create<bool, IConsole>(async (current, console) => {
                var profile = await Detection.DetectCliAsync(current);

                if (profile != null) {
                    console.Out.Write(profile.Shell + "\n");
                } else {
                    console.Error.Write($"Could not detect active shell profile\n");
                }
            });

            var getCommand = new Command("get-variable") {
                new Argument<string>("name", "Environment variable name"),
            };

            getCommand.Handler = CommandHandler.Create<string, IConsole>((name, console) => {
                Console.Out.WriteLine(System.Environment.GetEnvironmentVariable(name));
            });
            
            var setCommand = new Command("set-variable") {
                new Argument<string>("name", "Environment variable name"),
                new Argument<string>("value", "value"),
                new Option<bool>("--current", "Analyze the current process. (Default: false) to inspect the default shell for the OS")
            };

            setCommand.Handler = CommandHandler.Create<string, string, bool, IConsole>(async (name, value, current, console) => {
                var profile = await Detection.DetectCliAsync(current);

                if (profile != null) {
                    Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.User, profile);
                } else {
                    console.Error.Write($"Could not detect active shell profile\n");
                }
            });
 
            var rootCommand = new RootCommand
            {
                getCommand,
                detect,
                setCommand
            };

            rootCommand.Description = "Environment variable tool";

            // Note that the parameters of the handler method are matched according to the names of the options
            // rootCommand.Handler = CommandHandler.Create<int, bool, System.IO.FileInfo>((intOption, boolOption, fileOption) =>
            //     {
            //         Console.WriteLine($"The value for --int-option is: {intOption}");
            //         Console.WriteLine($"The value for --bool-option is: {boolOption}");
            //         Console.WriteLine($"The value for --file-option is: {fileOption?.FullName ?? "null"}");
            //     });

            return await rootCommand.InvokeAsync(args);
        }
    }
}
