using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.CommandLine.Rendering;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SqlBackupUtil
{
    internal class Startup
    {
        private readonly string[] _args;
        private readonly InvocationContext _invocationContext;
        private IConfiguration? _configuration;

        private IServiceProvider? _serviceProvider;

        internal Startup(InvocationContext invocationContext, string[]? args)
        {
            _invocationContext = invocationContext;
            _args = args ?? Array.Empty<string>();
        }

        private IConfiguration Configuration
            => _configuration ??= Configure();

        private IServiceProvider ServiceProvider =>
            _serviceProvider ??= ConfigureServices();

        internal Task<int> StartAsync()
        {
            var consoleRenderer = new ConsoleRenderer(
                _invocationContext.Console,
                OutputMode.Ansi,
                true);
            if (_invocationContext.Console is ITerminal terminal)
            {
                terminal.Clear();
            }
            return Commands
                .CreateBuilder(_invocationContext, consoleRenderer)
                .UseDefaults()
                .UseHost()
                .Build()
                .InvokeAsync(_args ?? Array.Empty<string>(), _invocationContext.Console);
        }

        private IConfiguration Configure()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true);
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLowerInvariant() == "development")
            {
                config.AddUserSecrets<Program>();
            }
            return config
                .AddEnvironmentVariables()
                .AddEnvironmentVariables("ASPNETCORE_")
                .AddEnvironmentVariables("NETCORE_")
                .AddEnvironmentVariables("DOTNET_")
                .AddCommandLine(_args)
                .Build();
        }

        private IServiceProvider ConfigureServices()
            => new ServiceCollection()
                .AddOptions()
                .Configure<SqlBackupSettings>(Configuration)
                .BuildServiceProvider();
    }
}