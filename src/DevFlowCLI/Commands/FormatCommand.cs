using CommandLine;
using DevFlow.CLI.Services;
using System;
using System.Threading.Tasks;

namespace DevFlow.CLI.Commands
{
    [Verb("format", HelpText = "Code formatting and quality tools")]
    public class FormatCommand : ICommand
    {
        [Option('p', "path", Required = false, HelpText = "Path to format", Default = ".")]
        public string Path { get; set; } = ".";

        [Option('v', "verify", Required = false, HelpText = "Verify formatting only", Default = false)]
        public bool VerifyOnly { get; set; }

        [Option('a', "analyze", Required = false, HelpText = "Run code analysis", Default = false)]
        public bool RunAnalysis { get; set; }

        public async Task ExecuteAsync()
        {
            Console.WriteLine($"Code formatting for path: {Path}");

            var codeQualityService = new CodeQualityService();

            if (VerifyOnly)
            {
                var isFormatted = await codeQualityService.VerifyFormattingAsync(Path);
                if (isFormatted)
                {
                    Console.WriteLine("✅ Code is properly formatted");
                }
                else
                {
                    Console.WriteLine("❌ Code formatting issues found");
                    throw new InvalidOperationException("Code formatting verification failed");
                }
            }
            else
            {
                await codeQualityService.FormatCodeAsync(Path);
                Console.WriteLine("✅ Code formatting completed");
            }

            if (RunAnalysis)
            {
                Console.WriteLine("Running code analysis...");
                await codeQualityService.RunAnalysisAsync(Path);
                Console.WriteLine("✅ Code analysis completed");
            }
        }
    }
}