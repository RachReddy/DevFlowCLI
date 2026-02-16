using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DevFlow.CLI.Services
{
    public class ConfigurationService
    {
        private readonly string _configDirectory;
        private readonly string _configFile;
        private DevFlowConfig? _config;

        public ConfigurationService()
        {
            _configDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".devflow");
            _configFile = Path.Combine(_configDirectory, "config.json");
        }

        public async Task<DevFlowConfig> GetConfigAsync()
        {
            if (_config == null)
            {
                _config = await LoadConfigAsync();
            }
            return _config;
        }

        public async Task SaveConfigAsync(DevFlowConfig config)
        {
            Directory.CreateDirectory(_configDirectory);
            
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(config, options);
            await File.WriteAllTextAsync(_configFile, json);
            
            _config = config;
        }

        public async Task InitializeConfigAsync()
        {
            var config = await GetConfigAsync();
            
            // Set default values if not already configured
            if (string.IsNullOrEmpty(config.DefaultAuthor))
            {
                config.DefaultAuthor = Environment.UserName;
            }

            if (string.IsNullOrEmpty(config.DefaultNamespace))
            {
                config.DefaultNamespace = "MyCompany";
            }

            if (config.Templates.Count == 0)
            {
                config.Templates = GetDefaultTemplates();
            }

            await SaveConfigAsync(config);
        }

        public async Task SetDefaultAuthorAsync(string author)
        {
            var config = await GetConfigAsync();
            config.DefaultAuthor = author;
            await SaveConfigAsync(config);
        }

        public async Task SetDefaultNamespaceAsync(string namespaceName)
        {
            var config = await GetConfigAsync();
            config.DefaultNamespace = namespaceName;
            await SaveConfigAsync(config);
        }

        public async Task AddCustomTemplateAsync(string name, string path)
        {
            var config = await GetConfigAsync();
            config.Templates[name] = new TemplateConfig
            {
                Name = name,
                Path = path,
                Type = "custom",
                Description = $"Custom template: {name}"
            };
            await SaveConfigAsync(config);
        }

        public async Task RemoveCustomTemplateAsync(string name)
        {
            var config = await GetConfigAsync();
            config.Templates.Remove(name);
            await SaveConfigAsync(config);
        }

        public async Task SetGitConfigAsync(string defaultBranch, bool autoCommit, bool autoPush)
        {
            var config = await GetConfigAsync();
            config.Git.DefaultBranch = defaultBranch;
            config.Git.AutoCommit = autoCommit;
            config.Git.AutoPush = autoPush;
            await SaveConfigAsync(config);
        }

        private async Task<DevFlowConfig> LoadConfigAsync()
        {
            if (!File.Exists(_configFile))
            {
                return new DevFlowConfig();
            }

            try
            {
                var json = await File.ReadAllTextAsync(_configFile);
                return JsonSerializer.Deserialize<DevFlowConfig>(json) ?? new DevFlowConfig();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not load configuration: {ex.Message}");
                return new DevFlowConfig();
            }
        }

        private Dictionary<string, TemplateConfig> GetDefaultTemplates()
        {
            return new Dictionary<string, TemplateConfig>
            {
                ["api"] = new TemplateConfig
                {
                    Name = "api",
                    Type = "builtin",
                    Description = "ASP.NET Core Web API with Swagger, logging, and Docker support"
                },
                ["web"] = new TemplateConfig
                {
                    Name = "web",
                    Type = "builtin",
                    Description = "ASP.NET Core MVC web application with Bootstrap and Docker support"
                },
                ["console"] = new TemplateConfig
                {
                    Name = "console",
                    Type = "builtin",
                    Description = "Simple .NET console application"
                }
            };
        }
    }

    public class DevFlowConfig
    {
        public string DefaultAuthor { get; set; } = string.Empty;
        public string DefaultNamespace { get; set; } = string.Empty;
        public Dictionary<string, TemplateConfig> Templates { get; set; } = new();
        public GitConfig Git { get; set; } = new();
        public QualityConfig Quality { get; set; } = new();
        public DockerConfig Docker { get; set; } = new();
    }

    public class TemplateConfig
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "builtin" or "custom"
        public string Path { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class GitConfig
    {
        public string DefaultBranch { get; set; } = "main";
        public bool AutoCommit { get; set; } = false;
        public bool AutoPush { get; set; } = true;
        public string CommitMessageTemplate { get; set; } = "{type}: {description}";
    }

    public class QualityConfig
    {
        public bool AutoFormat { get; set; } = true;
        public bool RunAnalysis { get; set; } = true;
        public bool EnforceStyleCop { get; set; } = false;
        public string[] ExcludedPaths { get; set; } = Array.Empty<string>();
    }

    public class DockerConfig
    {
        public string BaseImage { get; set; } = "mcr.microsoft.com/dotnet/aspnet:8.0";
        public string SdkImage { get; set; } = "mcr.microsoft.com/dotnet/sdk:8.0";
        public bool MultiStage { get; set; } = true;
        public string[] ExposedPorts { get; set; } = new[] { "8080", "8081" };
    }
}