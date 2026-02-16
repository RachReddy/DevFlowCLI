# DevFlow CLI - Development Workflow Automation Tool

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

DevFlow CLI is a powerful command-line tool designed to streamline development workflows for .NET projects. It provides project scaffolding, environment management, Git workflow automation, and code quality tools in a single, easy-to-use interface.

## 🚀 Features

- **Project Scaffolding**: Create new ASP.NET Core API, Web, and Console projects with best practices
- **Environment Management**: Set up and manage development, staging, and production environments
- **Git Workflow Automation**: Streamline feature branch creation, releases, and hotfixes
- **Code Quality Tools**: Integrated formatting, analysis, and security scanning
- **Docker Support**: Automatic Dockerfile generation for containerized deployments
- **Configuration Management**: Centralized environment variable and configuration handling

## 📦 Installation

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- Git (for Git workflow features)
- Docker (optional, for containerization features)

### Install as Global Tool

```bash
# Install from NuGet (when published)
dotnet tool install -g DevFlow.CLI

# Or install from source
git clone https://github.com/your-org/devflow-cli.git
cd devflow-cli/src/DevFlow.CLI
dotnet pack
dotnet tool install -g --add-source ./bin/Debug DevFlow.CLI
```

### Build from Source

```bash
git clone https://github.com/your-org/devflow-cli.git
cd devflow-cli/src/DevFlow.CLI
dotnet build
dotnet run -- --help
```

## 🛠️ Usage

### Project Scaffolding

Create new projects with best practices and modern tooling:

```bash
# Create a new ASP.NET Core API project
devflow new api MyAPI

# Create a new ASP.NET Core Web project
devflow new web MyWebApp

# Create a new Console application
devflow new console MyConsoleApp

# Create project in specific directory
devflow new api MyAPI --output ./projects/MyAPI

# Force overwrite existing directory
devflow new api MyAPI --force
```

#### Generated Project Features

**API Projects include:**
- ASP.NET Core 8.0 with minimal APIs
- Swagger/OpenAPI documentation
- Serilog structured logging
- Health checks endpoint
- Docker support with multi-stage builds
- Best practice project structure

**Web Projects include:**
- ASP.NET Core MVC with Razor views
- Bootstrap CSS framework
- Serilog structured logging
- Static file serving
- Docker support

### Environment Management

Manage environment configurations and variables:

```bash
# Set up development environment
devflow env setup dev

# Set up production environment with config file
devflow env setup prod --config ./config/prod.json

# Clean up environment
devflow env clean dev

# List all configured environments
devflow env list
```

#### Environment Configuration

DevFlow supports multiple configuration formats:

**JSON Configuration:**
```json
{
  "DatabaseConnection": "Server=localhost;Database=MyApp;",
  "ApiKey": "your-api-key",
  "LogLevel": "Information"
}
```

**Environment File (.env):**
```env
DATABASE_CONNECTION=Server=localhost;Database=MyApp;
API_KEY=your-api-key
LOG_LEVEL=Information
```

### Git Workflow Automation

Streamline your Git workflows:

```bash
# Create a new feature branch
devflow git feature user-authentication

# Create a release branch with tag
devflow git release 1.2.0 --message "Release version 1.2.0"

# Create a hotfix branch
devflow git hotfix critical-security-fix

# Skip pushing to remote
devflow git feature new-feature --push false
```

### Code Quality Tools

Maintain code quality and consistency:

```bash
# Format code in current directory
devflow format

# Format specific path
devflow format --path ./src

# Verify formatting without making changes
devflow format --verify

# Run code analysis
devflow format --analyze

# Format and analyze together
devflow format --path ./src --analyze
```

## 📁 Project Structure

```
DevFlowCLI/
├── src/DevFlow.CLI/
│   ├── Commands/           # CLI command implementations
│   │   ├── NewCommand.cs
│   │   ├── EnvCommand.cs
│   │   ├── GitCommand.cs
│   │   └── FormatCommand.cs
│   ├── Services/           # Core business logic
│   │   ├── TemplateService.cs
│   │   ├── EnvironmentService.cs
│   │   ├── GitService.cs
│   │   └── CodeQualityService.cs
│   ├── Templates/          # Project templates
│   └── Program.cs          # Application entry point
├── docker/                 # Docker configurations
├── .github/workflows/      # GitHub Actions
├── docs/                   # Documentation
└── README.md
```

## 🔧 Configuration

DevFlow CLI stores configuration in `~/.devflow/`:

- `environments.json` - Environment configurations
- `{env-name}.env` - Environment variable files
- `set-{env-name}.ps1` - PowerShell environment scripts
- `set-{env-name}.bat` - Batch environment scripts

## 🐳 Docker Support

All generated projects include Docker support:

```bash
# Build Docker image
docker build -t myapi .

# Run container
docker run -p 8080:8080 myapi

# Docker Compose (if generated)
docker-compose up
```

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md) for details.

### Development Setup

1. Clone the repository
2. Install .NET 8.0 SDK
3. Run `dotnet restore`
4. Run `dotnet build`
5. Run tests with `dotnet test`

### Code Style

- Follow C# coding conventions
- Use EditorConfig settings
- Run `dotnet format` before committing
- Ensure all tests pass

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

- 📖 [Documentation](docs/)
- 🐛 [Issue Tracker](https://github.com/your-org/devflow-cli/issues)
- 💬 [Discussions](https://github.com/your-org/devflow-cli/discussions)

## 🗺️ Roadmap

- [ ] NuGet package publishing
- [ ] Azure DevOps integration
- [ ] Kubernetes deployment templates
- [ ] Database migration tools
- [ ] Testing framework integration
- [ ] CI/CD pipeline templates

## 📊 Examples

### Quick Start

```bash
# Create a new API project
devflow new api TodoAPI

# Navigate to project
cd TodoAPI

# Set up development environment
devflow env setup dev

# Create feature branch
devflow git feature add-todo-endpoints

# Format code
devflow format

# Build and run
dotnet run
```

### Advanced Usage

```bash
# Create production-ready API with custom configuration
devflow new api ProductionAPI --output ./apps/production
cd ./apps/production

# Set up production environment with external config
devflow env setup prod --config ./config/production.json

# Create release branch
devflow git release 2.0.0 --message "Major release with new features"

# Run comprehensive code analysis
devflow format --path . --analyze --verify
```

---

**DevFlow CLI** - Streamlining development workflows, one command at a time. 🚀
