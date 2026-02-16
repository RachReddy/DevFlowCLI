using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DevFlow.CLI.Services
{
    public class GitService
    {
        public async Task CreateFeatureBranchAsync(string featureName, bool push)
        {
            Console.WriteLine($"Creating feature branch: feature/{featureName}");

            // Ensure we're on main/master branch
            await ExecuteGitCommandAsync("checkout main", ignoreErrors: true);
            await ExecuteGitCommandAsync("checkout master", ignoreErrors: true);

            // Pull latest changes
            await ExecuteGitCommandAsync("pull origin main", ignoreErrors: true);
            await ExecuteGitCommandAsync("pull origin master", ignoreErrors: true);

            // Create and checkout feature branch
            await ExecuteGitCommandAsync($"checkout -b feature/{featureName}");

            if (push)
            {
                // Push branch to remote
                await ExecuteGitCommandAsync($"push -u origin feature/{featureName}");
                Console.WriteLine($"Feature branch 'feature/{featureName}' pushed to remote");
            }

            Console.WriteLine($"Switched to feature branch: feature/{featureName}");
        }

        public async Task CreateReleaseAsync(string version, string message, bool push)
        {
            Console.WriteLine($"Creating release: {version}");

            // Ensure we're on main/master branch
            await ExecuteGitCommandAsync("checkout main", ignoreErrors: true);
            await ExecuteGitCommandAsync("checkout master", ignoreErrors: true);

            // Pull latest changes
            await ExecuteGitCommandAsync("pull origin main", ignoreErrors: true);
            await ExecuteGitCommandAsync("pull origin master", ignoreErrors: true);

            // Create release branch
            await ExecuteGitCommandAsync($"checkout -b release/{version}");

            // Create tag
            var tagMessage = string.IsNullOrEmpty(message) ? $"Release {version}" : message;
            await ExecuteGitCommandAsync($"tag -a v{version} -m \"{tagMessage}\"");

            if (push)
            {
                // Push branch and tag to remote
                await ExecuteGitCommandAsync($"push -u origin release/{version}");
                await ExecuteGitCommandAsync($"push origin v{version}");
                Console.WriteLine($"Release '{version}' pushed to remote with tag");
            }

            Console.WriteLine($"Release branch 'release/{version}' created with tag 'v{version}'");
        }

        public async Task CreateHotfixAsync(string hotfixName, bool push)
        {
            Console.WriteLine($"Creating hotfix branch: hotfix/{hotfixName}");

            // Ensure we're on main/master branch
            await ExecuteGitCommandAsync("checkout main", ignoreErrors: true);
            await ExecuteGitCommandAsync("checkout master", ignoreErrors: true);

            // Pull latest changes
            await ExecuteGitCommandAsync("pull origin main", ignoreErrors: true);
            await ExecuteGitCommandAsync("pull origin master", ignoreErrors: true);

            // Create and checkout hotfix branch
            await ExecuteGitCommandAsync($"checkout -b hotfix/{hotfixName}");

            if (push)
            {
                // Push branch to remote
                await ExecuteGitCommandAsync($"push -u origin hotfix/{hotfixName}");
                Console.WriteLine($"Hotfix branch 'hotfix/{hotfixName}' pushed to remote");
            }

            Console.WriteLine($"Switched to hotfix branch: hotfix/{hotfixName}");
        }

        private async Task ExecuteGitCommandAsync(string arguments, bool ignoreErrors = false)
        {
            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(processStartInfo);
                if (process != null)
                {
                    await process.WaitForExitAsync();
                    
                    var output = await process.StandardOutput.ReadToEndAsync();
                    var error = await process.StandardError.ReadToEndAsync();

                    if (process.ExitCode != 0 && !ignoreErrors)
                    {
                        throw new InvalidOperationException($"Git command failed: {arguments}\nError: {error}");
                    }

                    if (!string.IsNullOrWhiteSpace(output))
                    {
                        Console.WriteLine($"Git: {output.Trim()}");
                    }
                }
            }
            catch (Exception ex) when (ignoreErrors)
            {
                // Silently ignore errors when ignoreErrors is true
                Console.WriteLine($"Git command ignored: {arguments} ({ex.Message})");
            }
        }
    }
}