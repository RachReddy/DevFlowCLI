using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DevFlow.CLI.Services
{
    public class CodeQualityService
    {
        public async Task FormatCodeAsync(string path)
        {
            Console.WriteLine($"Formatting code in: {path}");

            // Run dotnet format
            await ExecuteDotNetCommandAsync($"format \"{path}\"");
            
            Console.WriteLine("Code formatting completed");
        }

        public async Task<bool> VerifyFormattingAsync(string path)
        {
            Console.WriteLine($"Verifying code formatting in: {path}");

            try
            {
                // Run dotnet format with --verify-no-changes flag
                await ExecuteDotNetCommandAsync($"format \"{path}\" --verify-no-changes");
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public async Task RunAnalysisAsync(string path)
        {
            Console.WriteLine($"Running code analysis in: {path}");

            // Find all .csproj files in the path
            var projectFiles = Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories);

            if (projectFiles.Length == 0)
            {
                Console.WriteLine("No .csproj files found for analysis");
                return;
            }

            foreach (var projectFile in projectFiles)
            {
                Console.WriteLine($"Analyzing project: {Path.GetFileName(projectFile)}");
                
                // Run dotnet build with analysis
                await ExecuteDotNetCommandAsync($"build \"{projectFile}\" --configuration Release --verbosity quiet");
                
                // Run security analysis if available
                await RunSecurityAnalysisAsync(projectFile);
            }

            Console.WriteLine("Code analysis completed");
        }

        private async Task RunSecurityAnalysisAsync(string projectFile)
        {
            try
            {
                Console.WriteLine("Running security analysis...");
                
                // Check if security analysis tools are available
                var projectDir = Path.GetDirectoryName(projectFile);
                if (projectDir != null)
                {
                    // Run dotnet list package --vulnerable
                    await ExecuteDotNetCommandAsync($"list \"{projectFile}\" package --vulnerable", ignoreErrors: true);
                    
                    // Run dotnet list package --deprecated
                    await ExecuteDotNetCommandAsync($"list \"{projectFile}\" package --deprecated", ignoreErrors: true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Security analysis warning: {ex.Message}");
            }
        }

        public async Task InstallCodeAnalysisToolsAsync()
        {
            Console.WriteLine("Installing code analysis tools...");

            try
            {
                // Install dotnet format as global tool
                await ExecuteDotNetCommandAsync("tool install -g dotnet-format", ignoreErrors: true);
                
                // Install security analysis tools
                await ExecuteDotNetCommandAsync("tool install -g security-scan", ignoreErrors: true);
                
                Console.WriteLine("Code analysis tools installed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not install some tools: {ex.Message}");
            }
        }

        public async Task CreateEditorConfigAsync(string path)
        {
            Console.WriteLine("Creating .editorconfig file...");

            var editorConfigPath = Path.Combine(path, ".editorconfig");
            var editorConfigContent = GetEditorConfigContent();
            
            await File.WriteAllTextAsync(editorConfigPath, editorConfigContent);
            Console.WriteLine($".editorconfig created at: {editorConfigPath}");
        }

        private async Task ExecuteDotNetCommandAsync(string arguments, bool ignoreErrors = false)
        {
            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
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
                        throw new InvalidOperationException($"dotnet command failed: {arguments}\nError: {error}");
                    }

                    if (!string.IsNullOrWhiteSpace(output))
                    {
                        Console.WriteLine(output.Trim());
                    }

                    if (!string.IsNullOrWhiteSpace(error) && !ignoreErrors)
                    {
                        Console.WriteLine($"Warning: {error.Trim()}");
                    }
                }
            }
            catch (Exception ex) when (ignoreErrors)
            {
                Console.WriteLine($"Command ignored: dotnet {arguments} ({ex.Message})");
            }
        }

        private string GetEditorConfigContent()
        {
            return @"root = true

[*]
charset = utf-8
end_of_line = crlf
insert_final_newline = true
indent_style = space
indent_size = 4
trim_trailing_whitespace = true

[*.{cs,csx,vb,vbx}]
indent_size = 4

[*.{json,js,ts,html,css,scss,less}]
indent_size = 2

[*.{yml,yaml}]
indent_size = 2

[*.md]
trim_trailing_whitespace = false

# C# formatting rules
[*.cs]
# Organize usings
dotnet_sort_system_directives_first = true
dotnet_separate_import_directive_groups = false

# this. preferences
dotnet_style_qualification_for_field = false:suggestion
dotnet_style_qualification_for_property = false:suggestion
dotnet_style_qualification_for_method = false:suggestion
dotnet_style_qualification_for_event = false:suggestion

# Language keywords vs BCL types preferences
dotnet_style_predefined_type_for_locals_parameters_members = true:suggestion
dotnet_style_predefined_type_for_member_access = true:suggestion

# Parentheses preferences
dotnet_style_parentheses_in_arithmetic_binary_operators = always_for_clarity:silent
dotnet_style_parentheses_in_relational_binary_operators = always_for_clarity:silent
dotnet_style_parentheses_in_other_binary_operators = always_for_clarity:silent
dotnet_style_parentheses_in_other_operators = never_if_unnecessary:silent

# Modifier preferences
dotnet_style_require_accessibility_modifiers = for_non_interface_members:suggestion
dotnet_style_readonly_field = true:suggestion

# Expression-level preferences
dotnet_style_object_initializer = true:suggestion
dotnet_style_collection_initializer = true:suggestion
dotnet_style_explicit_tuple_names = true:suggestion
dotnet_style_null_propagation = true:suggestion
dotnet_style_coalesce_expression = true:suggestion
dotnet_style_prefer_is_null_check_over_reference_equality_method = true:suggestion
dotnet_style_prefer_inferred_tuple_names = true:suggestion
dotnet_style_prefer_inferred_anonymous_type_member_names = true:suggestion
dotnet_style_prefer_auto_properties = true:silent
dotnet_style_prefer_conditional_expression_over_assignment = true:silent
dotnet_style_prefer_conditional_expression_over_return = true:silent

# C# formatting rules
csharp_new_line_before_open_brace = all
csharp_new_line_before_else = true
csharp_new_line_before_catch = true
csharp_new_line_before_finally = true
csharp_new_line_before_members_in_object_initializers = true
csharp_new_line_before_members_in_anonymous_types = true
csharp_new_line_between_query_expression_clauses = true

# Indentation preferences
csharp_indent_case_contents = true
csharp_indent_switch_labels = true
csharp_indent_labels = flush_left

# Space preferences
csharp_space_after_cast = false
csharp_space_after_keywords_in_control_flow_statements = true
csharp_space_between_method_call_parameter_list_parentheses = false
csharp_space_between_method_declaration_parameter_list_parentheses = false
csharp_space_between_parentheses = false
csharp_space_before_colon_in_inheritance_clause = true
csharp_space_after_colon_in_inheritance_clause = true
csharp_space_around_binary_operators = before_and_after
csharp_space_between_method_declaration_empty_parameter_list_parentheses = false
csharp_space_between_method_call_name_and_opening_parenthesis = false
csharp_space_between_method_call_empty_parameter_list_parentheses = false

# Wrapping preferences
csharp_preserve_single_line_statements = true
csharp_preserve_single_line_blocks = true

# var preferences
csharp_style_var_for_built_in_types = false:suggestion
csharp_style_var_when_type_is_apparent = true:suggestion
csharp_style_var_elsewhere = false:suggestion

# Expression-bodied members
csharp_style_expression_bodied_methods = false:silent
csharp_style_expression_bodied_constructors = false:silent
csharp_style_expression_bodied_operators = false:silent
csharp_style_expression_bodied_properties = true:silent
csharp_style_expression_bodied_indexers = true:silent
csharp_style_expression_bodied_accessors = true:silent

# Pattern matching preferences
csharp_style_pattern_matching_over_is_with_cast_check = true:suggestion
csharp_style_pattern_matching_over_as_with_null_check = true:suggestion

# Null-checking preferences
csharp_style_throw_expression = true:suggestion
csharp_style_conditional_delegate_call = true:suggestion

# Modifier preferences
csharp_preferred_modifier_order = public,private,protected,internal,static,extern,new,virtual,abstract,sealed,override,readonly,unsafe,volatile,async:suggestion
";
        }
    }
}
