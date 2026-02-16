using System;
using System.IO;
using System.Threading.Tasks;

namespace DevFlow.CLI.Services
{
    public class TemplateService
    {
        public async Task CreateApiProjectAsync(string projectName, string outputDirectory)
        {
            Console.WriteLine("Creating ASP.NET Core API project...");
            
            // Create directory structure
            Directory.CreateDirectory(outputDirectory);
            Directory.CreateDirectory(Path.Combine(outputDirectory, "Controllers"));
            Directory.CreateDirectory(Path.Combine(outputDirectory, "Models"));
            Directory.CreateDirectory(Path.Combine(outputDirectory, "Services"));
            Directory.CreateDirectory(Path.Combine(outputDirectory, "Properties"));

            // Create project file
            var projectContent = GetApiProjectFileContent(projectName);
            await File.WriteAllTextAsync(Path.Combine(outputDirectory, $"{projectName}.csproj"), projectContent);

            // Create Program.cs
            var programContent = GetApiProgramContent();
            await File.WriteAllTextAsync(Path.Combine(outputDirectory, "Program.cs"), programContent);

            // Create appsettings.json
            var appSettingsContent = GetAppSettingsContent();
            await File.WriteAllTextAsync(Path.Combine(outputDirectory, "appsettings.json"), appSettingsContent);

            // Create appsettings.Development.json
            var devAppSettingsContent = GetDevAppSettingsContent();
            await File.WriteAllTextAsync(Path.Combine(outputDirectory, "appsettings.Development.json"), devAppSettingsContent);

            // Create sample controller
            var controllerContent = GetSampleControllerContent(projectName);
            await File.WriteAllTextAsync(Path.Combine(outputDirectory, "Controllers", "WeatherForecastController.cs"), controllerContent);

            // Create sample model
            var modelContent = GetWeatherForecastModelContent(projectName);
            await File.WriteAllTextAsync(Path.Combine(outputDirectory, "Models", "WeatherForecast.cs"), modelContent);

            // Create Dockerfile
            var dockerfileContent = GetApiDockerfileContent(projectName);
            await File.WriteAllTextAsync(Path.Combine(outputDirectory, "Dockerfile"), dockerfileContent);

            // Create .dockerignore
            var dockerIgnoreContent = GetDockerIgnoreContent();
            await File.WriteAllTextAsync(Path.Combine(outputDirectory, ".dockerignore"), dockerIgnoreContent);

            // Create launchSettings.json
            var launchSettingsContent = GetLaunchSettingsContent();
            await File.WriteAllTextAsync(Path.Combine(outputDirectory, "Properties", "launchSettings.json"), launchSettingsContent);
        }

        public async Task CreateWebProjectAsync(string projectName, string outputDirectory)
        {
            Console.WriteLine("Creating ASP.NET Core Web project...");
            
            // Create directory structure
            Directory.CreateDirectory(outputDirectory);
            Directory.CreateDirectory(Path.Combine(outputDirectory, "Controllers"));
            Directory.CreateDirectory(Path.Combine(outputDirectory, "Views"));
            Directory.CreateDirectory(Path.Combine(outputDirectory, "Views", "Home"));
            Directory.CreateDirectory(Path.Combine(outputDirectory, "Views", "Shared"));
            Directory.CreateDirectory(Path.Combine(outputDirectory, "wwwroot"));
            Directory.CreateDirectory(Path.Combine(outputDirectory, "wwwroot", "css"));
            Directory.CreateDirectory(Path.Combine(outputDirectory, "wwwroot", "js"));
            Directory.CreateDirectory(Path.Combine(outputDirectory, "Models"));
            Directory.CreateDirectory(Path.Combine(outputDirectory, "Properties"));

            // Create project file
            var projectContent = GetWebProjectFileContent(projectName);
            await File.WriteAllTextAsync(Path.Combine(outputDirectory, $"{projectName}.csproj"), projectContent);

            // Create Program.cs
            var programContent = GetWebProgramContent();
            await File.WriteAllTextAsync(Path.Combine(outputDirectory, "Program.cs"), programContent);

            // Create HomeController
            var homeControllerContent = GetHomeControllerContent(projectName);
            await File.WriteAllTextAsync(Path.Combine(outputDirectory, "Controllers", "HomeController.cs"), homeControllerContent);

            // Create basic views and other web assets
            await CreateWebViewsAndAssets(outputDirectory, projectName);

            // Create Dockerfile
            var dockerfileContent = GetWebDockerfileContent(projectName);
            await File.WriteAllTextAsync(Path.Combine(outputDirectory, "Dockerfile"), dockerfileContent);
        }

        public async Task CreateConsoleProjectAsync(string projectName, string outputDirectory)
        {
            Console.WriteLine("Creating Console project...");
            
            Directory.CreateDirectory(outputDirectory);

            // Create project file
            var projectContent = GetConsoleProjectFileContent(projectName);
            await File.WriteAllTextAsync(Path.Combine(outputDirectory, $"{projectName}.csproj"), projectContent);

            // Create Program.cs
            var programContent = GetConsoleProgramContent(projectName);
            await File.WriteAllTextAsync(Path.Combine(outputDirectory, "Program.cs"), programContent);
        }

        private async Task CreateWebViewsAndAssets(string outputDirectory, string projectName)
        {
            // Create _Layout.cshtml
            var layoutContent = GetLayoutContent(projectName);
            await File.WriteAllTextAsync(Path.Combine(outputDirectory, "Views", "Shared", "_Layout.cshtml"), layoutContent);

            // Create Index.cshtml
            var indexContent = GetIndexViewContent();
            await File.WriteAllTextAsync(Path.Combine(outputDirectory, "Views", "Home", "Index.cshtml"), indexContent);

            // Create basic CSS
            var cssContent = GetBasicCssContent();
            await File.WriteAllTextAsync(Path.Combine(outputDirectory, "wwwroot", "css", "site.css"), cssContent);
        }

        #region Template Content Methods

        private string GetApiProjectFileContent(string projectName)
        {
            return $@"<Project Sdk=""Microsoft.NET.Sdk.Web"">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <RootNamespace>{projectName}</RootNamespace>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Microsoft.AspNetCore.OpenApi"" Version=""8.0.0"" />
    <PackageReference Include=""Swashbuckle.AspNetCore"" Version=""6.4.0"" />
    <PackageReference Include=""Serilog.AspNetCore"" Version=""8.0.0"" />
    <PackageReference Include=""Serilog.Sinks.Console"" Version=""5.0.0"" />
    <PackageReference Include=""Microsoft.AspNetCore.HealthChecks"" Version=""1.0.0"" />
  </ItemGroup>

</Project>";
        }

        private string GetWebProjectFileContent(string projectName)
        {
            return $@"<Project Sdk=""Microsoft.NET.Sdk.Web"">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <RootNamespace>{projectName}</RootNamespace>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Serilog.AspNetCore"" Version=""8.0.0"" />
    <PackageReference Include=""Serilog.Sinks.Console"" Version=""5.0.0"" />
  </ItemGroup>

</Project>";
        }

        private string GetConsoleProjectFileContent(string projectName)
        {
            return $@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <RootNamespace>{projectName}</RootNamespace>
  </PropertyGroup>

</Project>";
        }

        private string GetApiProgramContent()
        {
            return @"using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Add health check endpoint
app.MapHealthChecks(""/health"");

app.Run();";
        }

        private string GetWebProgramContent()
        {
            return @"using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(""/Home/Error"");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: ""default"",
    pattern: ""{controller=Home}/{action=Index}/{id?}"");

app.Run();";
        }

        private string GetConsoleProgramContent(string projectName)
        {
            return $@"using System;

namespace {projectName}
{{
    class Program
    {{
        static void Main(string[] args)
        {{
            Console.WriteLine(""Hello from {projectName}!"");
            Console.WriteLine(""Arguments received: "" + string.Join("", "", args));
        }}
    }}
}}";
        }

        private string GetAppSettingsContent()
        {
            return @"{
  ""Logging"": {
    ""LogLevel"": {
      ""Default"": ""Information"",
      ""Microsoft.AspNetCore"": ""Warning""
    }
  },
  ""AllowedHosts"": ""*"",
  ""Serilog"": {
    ""Using"": [""Serilog.Sinks.Console""],
    ""MinimumLevel"": ""Information"",
    ""WriteTo"": [
      {
        ""Name"": ""Console"",
        ""Args"": {
          ""outputTemplate"": ""[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}""
        }
      }
    ]
  }
}";
        }

        private string GetDevAppSettingsContent()
        {
            return @"{
  ""Logging"": {
    ""LogLevel"": {
      ""Default"": ""Debug"",
      ""System"": ""Information"",
      ""Microsoft"": ""Information""
    }
  },
  ""Serilog"": {
    ""MinimumLevel"": ""Debug""
  }
}";
        }

        private string GetSampleControllerContent(string projectName)
        {
            return $@"using Microsoft.AspNetCore.Mvc;
using {projectName}.Models;

namespace {projectName}.Controllers
{{
    [ApiController]
    [Route(""[controller]"")]
    public class WeatherForecastController : ControllerBase
    {{
        private static readonly string[] Summaries = new[]
        {{
            ""Freezing"", ""Bracing"", ""Chilly"", ""Cool"", ""Mild"", ""Warm"", ""Balmy"", ""Hot"", ""Sweltering"", ""Scorching""
        }};

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {{
            _logger = logger;
        }}

        [HttpGet(Name = ""GetWeatherForecast"")]
        public IEnumerable<WeatherForecast> Get()
        {{
            _logger.LogInformation(""Getting weather forecast"");
            
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {{
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            }})
            .ToArray();
        }}
    }}
}}";
        }

        private string GetWeatherForecastModelContent(string projectName)
        {
            return $@"namespace {projectName}.Models
{{
    public class WeatherForecast
    {{
        public DateOnly Date {{ get; set; }}

        public int TemperatureC {{ get; set; }}

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary {{ get; set; }}
    }}
}}";
        }

        private string GetHomeControllerContent(string projectName)
        {
            return $@"using Microsoft.AspNetCore.Mvc;

namespace {projectName}.Controllers
{{
    public class HomeController : Controller
    {{
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {{
            _logger = logger;
        }}

        public IActionResult Index()
        {{
            _logger.LogInformation(""Home page accessed"");
            return View();
        }}
    }}
}}";
        }

        private string GetLayoutContent(string projectName)
        {
            return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>@ViewData[""Title""] - {projectName}</title>
    <link rel=""stylesheet"" href=""~/css/site.css"" />
</head>
<body>
    <header>
        <nav class=""navbar"">
            <div class=""container"">
                <a class=""navbar-brand"" href=""/"">
                    {projectName}
                </a>
            </div>
        </nav>
    </header>
    <div class=""container"">
        <main role=""main"" class=""pb-3"">
            @RenderBody()
        </main>
    </div>

    <footer class=""footer"">
        <div class=""container"">
            &copy; 2024 - {projectName}
        </div>
    </footer>
</body>
</html>";
        }

        private string GetIndexViewContent()
        {
            return @"@{
    ViewData[""Title""] = ""Home Page"";
}

<div class=""text-center"">
    <h1 class=""display-4"">Welcome</h1>
    <p>Learn about <a href=""https://docs.microsoft.com/aspnet/core"">building Web apps with ASP.NET Core</a>.</p>
</div>";
        }

        private string GetBasicCssContent()
        {
            return @"html {
  font-size: 14px;
}

@media (min-width: 768px) {
  html {
    font-size: 16px;
  }
}

html {
  position: relative;
  min-height: 100%;
}

body {
  margin-bottom: 60px;
  font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, ""Helvetica Neue"", Arial, sans-serif;
}

.navbar {
  background-color: #343a40;
  padding: 1rem;
}

.navbar-brand {
  color: white;
  text-decoration: none;
  font-weight: bold;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 15px;
}

.footer {
  position: absolute;
  bottom: 0;
  width: 100%;
  white-space: nowrap;
  line-height: 60px;
  background-color: #f8f9fa;
  text-align: center;
}";
        }

        private string GetApiDockerfileContent(string projectName)
        {
            return $@"FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY [""{projectName}.csproj"", "".""]
RUN dotnet restore ""./{projectName}.csproj""
COPY . .
WORKDIR ""/src/.""
RUN dotnet build ""{projectName}.csproj"" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish ""{projectName}.csproj"" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [""dotnet"", ""{projectName}.dll""]";
        }

        private string GetWebDockerfileContent(string projectName)
        {
            return $@"FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY [""{projectName}.csproj"", "".""]
RUN dotnet restore ""./{projectName}.csproj""
COPY . .
WORKDIR ""/src/.""
RUN dotnet build ""{projectName}.csproj"" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish ""{projectName}.csproj"" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [""dotnet"", ""{projectName}.dll""]";
        }

        private string GetDockerIgnoreContent()
        {
            return @"**/.dockerignore
**/.env
**/.git
**/.gitignore
**/.project
**/.settings
**/.toolstarget
**/.vs
**/.vscode
**/.idea
**/*.*proj.user
**/*.dbmdl
**/*.jfm
**/azds.yaml
**/bin
**/charts
**/docker-compose*
**/Dockerfile*
**/node_modules
**/npm-debug.log
**/obj
**/secrets.dev.yaml
**/values.dev.yaml
LICENSE
README.md";
        }

        private string GetLaunchSettingsContent()
        {
            return @"{
  ""$schema"": ""http://json.schemastore.org/launchsettings.json"",
  ""iisSettings"": {
    ""windowsAuthentication"": false,
    ""anonymousAuthentication"": true,
    ""iisExpress"": {
      ""applicationUrl"": ""http://localhost:5000"",
      ""sslPort"": 44300
    }
  },
  ""profiles"": {
    ""http"": {
      ""commandName"": ""Project"",
      ""dotnetRunMessages"": true,
      ""launchBrowser"": true,
      ""launchUrl"": ""swagger"",
      ""applicationUrl"": ""http://localhost:5000"",
      ""environmentVariables"": {
        ""ASPNETCORE_ENVIRONMENT"": ""Development""
      }
    },
    ""https"": {
      ""commandName"": ""Project"",
      ""dotnetRunMessages"": true,
      ""launchBrowser"": true,
      ""launchUrl"": ""swagger"",
      ""applicationUrl"": ""https://localhost:7000;http://localhost:5000"",
      ""environmentVariables"": {
        ""ASPNETCORE_ENVIRONMENT"": ""Development""
      }
    }
  }
}";
        }

        #endregion
    }
}