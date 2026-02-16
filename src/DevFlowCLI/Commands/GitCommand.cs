using CommandLine;
using DevFlow.CLI.Services;
using System;
using System.Threading.Tasks;

namespace DevFlow.CLI.Commands
{
    [Verb("git", HelpText = "Git workflow automation")]
    public class GitCommand : ICommand
    {
        [Value(0, MetaName = "action", Required = true, HelpText = "Action (feature, release, hotfix)")]
        public string Action { get; set; } = string.Empty;

        [Value(1, MetaName = "name", Required = false, HelpText = "Branch/release name")]
        public string Name { get; set; } = string.Empty;

        [Option('m', "message", Required = false, HelpText = "Commit message")]
        public string Message { get; set; } = string.Empty;

        [Option('p', "push", Required = false, HelpText = "Push to remote", Default = true)]
        public bool Push { get; set; } = true;

        public async Task ExecuteAsync()
        {
            Console.WriteLine($"Git workflow: {Action}");

            var gitService = new GitService();

            switch (Action.ToLower())
            {
                case "feature":
                    if (string.IsNullOrEmpty(Name))
                        throw new ArgumentException("Feature name is required");
                    await gitService.CreateFeatureBranchAsync(Name, Push);
                    Console.WriteLine($"✅ Feature branch '{Name}' created");
                    break;
                case "release":
                    if (string.IsNullOrEmpty(Name))
                        throw new ArgumentException("Release version is required");
                    await gitService.CreateReleaseAsync(Name, Message, Push);
                    Console.WriteLine($"✅ Release '{Name}' created");
                    break;
                case "hotfix":
                    if (string.IsNullOrEmpty(Name))
                        throw new ArgumentException("Hotfix name is required");
                    await gitService.CreateHotfixAsync(Name, Push);
                    Console.WriteLine($"✅ Hotfix branch '{Name}' created");
                    break;
                default:
                    throw new ArgumentException($"Unknown action: {Action}. Available actions: feature, release, hotfix");
            }
        }
    }
}