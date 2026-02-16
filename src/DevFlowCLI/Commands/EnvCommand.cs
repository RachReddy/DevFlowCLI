using CommandLine;
using DevFlow.CLI.Services;
using System;
using System.Threading.Tasks;

namespace DevFlow.CLI.Commands
{
    [Verb("env", HelpText = "Environment management commands")]
    public class EnvCommand : ICommand
    {
        [Value(0, MetaName = "action", Required = true, HelpText = "Action (setup, clean, list)")]
        public string Action { get; set; } = string.Empty;

        [Option('e', "environment", Required = false, HelpText = "Environment name (dev, staging, prod)", Default = "dev")]
        public string Environment { get; set; } = "dev";

        [Option('c', "config", Required = false, HelpText = "Configuration file path")]
        public string ConfigPath { get; set; } = string.Empty;

        public async Task ExecuteAsync()
        {
            Console.WriteLine($"Environment management: {Action} for {Environment}");

            var envService = new EnvironmentService();

            switch (Action.ToLower())
            {
                case "setup":
                    await envService.SetupEnvironmentAsync(Environment, ConfigPath);
                    Console.WriteLine($"✅ Environment '{Environment}' setup completed");
                    break;
                case "clean":
                    await envService.CleanEnvironmentAsync(Environment);
                    Console.WriteLine($"✅ Environment '{Environment}' cleaned");
                    break;
                case "list":
                    await envService.ListEnvironmentsAsync();
                    break;
                default:
                    throw new ArgumentException($"Unknown action: {Action}. Available actions: setup, clean, list");
            }
        }
    }
}