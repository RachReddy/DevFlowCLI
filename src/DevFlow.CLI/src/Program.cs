using CommandLine;
using DevFlow.CLI.Commands;
using System;
using System.Threading.Tasks;

namespace DevFlow.CLI
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            Console.WriteLine("DevFlow CLI - Development Workflow Automation Tool");
            Console.WriteLine("================================================");

            var result = await Parser.Default.ParseArguments<
                NewCommand,
                EnvCommand,
                GitCommand,
                FormatCommand
            >(args)
            .MapResult(
                (NewCommand opts) => HandleNewCommand(opts),
                (EnvCommand opts) => HandleEnvCommand(opts),
                (GitCommand opts) => HandleGitCommand(opts),
                (FormatCommand opts) => HandleFormatCommand(opts),
                errs => Task.FromResult(1)
            );

            return result;
        }

        private static async Task<int> HandleNewCommand(NewCommand command)
        {
            try
            {
                await command.ExecuteAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 1;
            }
        }

        private static async Task<int> HandleEnvCommand(EnvCommand command)
        {
            try
            {
                await command.ExecuteAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 1;
            }
        }

        private static async Task<int> HandleGitCommand(GitCommand command)
        {
            try
            {
                await command.ExecuteAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 1;
            }
        }

        private static async Task<int> HandleFormatCommand(FormatCommand command)
        {
            try
            {
                await command.ExecuteAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 1;
            }
        }
    }
}
