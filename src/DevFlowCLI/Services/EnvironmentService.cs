using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;

namespace DevFlow.CLI.Services
{
    public class EnvironmentService
    {
        private readonly string _configDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".devflow");
        private readonly string _environmentsFile = "environments.json";

        public async Task SetupEnvironmentAsync(string environmentName, string configPath)
        {
            Console.WriteLine($"Setting up environment: {environmentName}");

            // Ensure config directory exists
            Directory.CreateDirectory(_configDirectory);

            var environments = await LoadEnvironmentsAsync();

            // Create environment configuration
            var envConfig = new EnvironmentConfig
            {
                Name = environmentName,
                CreatedAt = DateTime.UtcNow,
                ConfigPath = configPath,
                Variables = new Dictionary<string, string>()
            };

            // Add default environment variables based on environment type
            switch (environmentName.ToLower())
            {
                case "dev":
                case "development":
                    envConfig.Variables["ASPNETCORE_ENVIRONMENT"] = "Development";
                    envConfig.Variables["DOTNET_ENVIRONMENT"] = "Development";
                    break;
                case "staging":
                    envConfig.Variables["ASPNETCORE_ENVIRONMENT"] = "Staging";
                    envConfig.Variables["DOTNET_ENVIRONMENT"] = "Staging";
                    break;
                case "prod":
                case "production":
                    envConfig.Variables["ASPNETCORE_ENVIRONMENT"] = "Production";
                    envConfig.Variables["DOTNET_ENVIRONMENT"] = "Production";
                    break;
            }

            // Load additional configuration from file if provided
            if (!string.IsNullOrEmpty(configPath) && File.Exists(configPath))
            {
                await LoadConfigurationFromFileAsync(envConfig, configPath);
            }

            environments[environmentName] = envConfig;
            await SaveEnvironmentsAsync(environments);

            // Create environment-specific files
            await CreateEnvironmentFilesAsync(environmentName, envConfig);

            Console.WriteLine($"Environment '{environmentName}' configured with {envConfig.Variables.Count} variables");
        }

        public async Task CleanEnvironmentAsync(string environmentName)
        {
            Console.WriteLine($"Cleaning environment: {environmentName}");

            var environments = await LoadEnvironmentsAsync();

            if (environments.ContainsKey(environmentName))
            {
                environments.Remove(environmentName);
                await SaveEnvironmentsAsync(environments);

                // Clean up environment-specific files
                var envFile = Path.Combine(_configDirectory, $"{environmentName}.env");
                if (File.Exists(envFile))
                {
                    File.Delete(envFile);
                }

                Console.WriteLine($"Environment '{environmentName}' cleaned up");
            }
            else
            {
                Console.WriteLine($"Environment '{environmentName}' not found");
            }
        }

        public async Task ListEnvironmentsAsync()
        {
            Console.WriteLine("Available environments:");
            Console.WriteLine("=====================");

            var environments = await LoadEnvironmentsAsync();

            if (environments.Count == 0)
            {
                Console.WriteLine("No environments configured");
                return;
            }

            foreach (var env in environments)
            {
                Console.WriteLine($"📁 {env.Key}");
                Console.WriteLine($"   Created: {env.Value.CreatedAt:yyyy-MM-dd HH:mm:ss} UTC");
                Console.WriteLine($"   Variables: {env.Value.Variables.Count}");
                if (!string.IsNullOrEmpty(env.Value.ConfigPath))
                {
                    Console.WriteLine($"   Config: {env.Value.ConfigPath}");
                }
                Console.WriteLine();
            }
        }

        private async Task<Dictionary<string, EnvironmentConfig>> LoadEnvironmentsAsync()
        {
            var filePath = Path.Combine(_configDirectory, _environmentsFile);
            
            if (!File.Exists(filePath))
            {
                return new Dictionary<string, EnvironmentConfig>();
            }

            try
            {
                var json = await File.ReadAllTextAsync(filePath);
                return JsonSerializer.Deserialize<Dictionary<string, EnvironmentConfig>>(json) 
                       ?? new Dictionary<string, EnvironmentConfig>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not load environments file: {ex.Message}");
                return new Dictionary<string, EnvironmentConfig>();
            }
        }

        private async Task SaveEnvironmentsAsync(Dictionary<string, EnvironmentConfig> environments)
        {
            var filePath = Path.Combine(_configDirectory, _environmentsFile);
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(environments, options);
            await File.WriteAllTextAsync(filePath, json);
        }

        private async Task LoadConfigurationFromFileAsync(EnvironmentConfig envConfig, string configPath)
        {
            try
            {
                var content = await File.ReadAllTextAsync(configPath);
                
                if (configPath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    var config = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
                    if (config != null)
                    {
                        foreach (var kvp in config)
                        {
                            envConfig.Variables[kvp.Key] = kvp.Value?.ToString() ?? string.Empty;
                        }
                    }
                }
                else if (configPath.EndsWith(".env", StringComparison.OrdinalIgnoreCase))
                {
                    var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        var trimmedLine = line.Trim();
                        if (!trimmedLine.StartsWith("#") && trimmedLine.Contains("="))
                        {
                            var parts = trimmedLine.Split('=', 2);
                            if (parts.Length == 2)
                            {
                                envConfig.Variables[parts[0].Trim()] = parts[1].Trim();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not load configuration from {configPath}: {ex.Message}");
            }
        }

        private async Task CreateEnvironmentFilesAsync(string environmentName, EnvironmentConfig envConfig)
        {
            // Create .env file
            var envFile = Path.Combine(_configDirectory, $"{environmentName}.env");
            var envContent = string.Join("\n", 
                envConfig.Variables.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            await File.WriteAllTextAsync(envFile, envContent);

            // Create PowerShell script for Windows
            var psFile = Path.Combine(_configDirectory, $"set-{environmentName}.ps1");
            var psContent = string.Join("\n", 
                envConfig.Variables.Select(kvp => $"$env:{kvp.Key}=\"{kvp.Value}\""));
            await File.WriteAllTextAsync(psFile, psContent);

            // Create batch file for Windows
            var batFile = Path.Combine(_configDirectory, $"set-{environmentName}.bat");
            var batContent = string.Join("\n", 
                envConfig.Variables.Select(kvp => $"set {kvp.Key}={kvp.Value}"));
            await File.WriteAllTextAsync(batFile, batContent);
        }
    }

    public class EnvironmentConfig
    {
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string ConfigPath { get; set; } = string.Empty;
        public Dictionary<string, string> Variables { get; set; } = new();
    }
}