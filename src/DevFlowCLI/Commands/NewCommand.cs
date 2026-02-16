using CommandLine;
using DevFlow.CLI.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DevFlow.CLI.Commands
{
    [Verb("new", HelpText = "Create a new project from template")]
    public class NewCommand : ICommand
    {
        [Value(0, MetaName = "template", Required = true, HelpText = "Template type (api, web, console)")]
        public string Template { get; set; } = string.Empty;

        [Value(1, MetaName = "name", Required = true, HelpText = "Project name")]
        public string ProjectName { get; set; } = string.Empty;

        [Option('o', "output", Required = false, HelpText = "Output directory")]
        public string OutputDirectory { get; set; } = string.Empty;

        [Option('f', "force", Required = false, HelpText = "Overwrite existing files")]
        public bool Force { get; set; }

        public async Task ExecuteAsync()
        {
            Console.WriteLine($"Creating new {Template} project: {ProjectName}");

            var outputDir = string.IsNullOrEmpty(OutputDirectory) ? ProjectName : OutputDirectory;
            
            if (Directory.Exists(outputDir) && !Force)
            {
                throw new InvalidOperationException($"Directory '{outputDir}' already exists. Use --force to overwrite.");
            }

            var templateService = new TemplateService();
            
            switch (Template.ToLower())
            {
                case "api":
                    await templateService.CreateApiProjectAsync(ProjectName, outputDir);
                    break;
                case "web":
                    await templateService.CreateWebProjectAsync(ProjectName, outputDir);
                    break;
                case "console":
                    await templateService.CreateConsoleProjectAsync(ProjectName, outputDir);
                    break;
                default:
                    throw new ArgumentException($"Unknown template: {Template}. Available templates: api, web, console");
            }

            Console.WriteLine($"✅ Project '{ProjectName}' created successfully in '{outputDir}'");
            Console.WriteLine("\nNext steps:");
            Console.WriteLine($"  cd {outputDir}");
            Console.WriteLine("  dotnet restore");
            Console.WriteLine("  dotnet run");
        }
    }
}