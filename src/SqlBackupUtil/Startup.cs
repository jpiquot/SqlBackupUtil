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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

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
            if (_invocationContext.Console is ITerminal terminal)
            {
                terminal.Clear();
            }
            return new CommandLineBuilder(
                new MainCommand(
                        Options.Create
                        (
                            Configuration.GetValue<SqlBackupSettings>(nameof(SqlBackupSettings))
                        )))
                .UseDefaults()
                .UseHost(_ => Host
                    .CreateDefaultBuilder()
                    .ConfigureAppConfiguration((builder) => Configure(builder))
                    .ConfigureHostConfiguration((builder) => Configure(builder))
                    .ConfigureServices((services) => ConfigureServices(services)))
                .Build()
                .InvokeAsync(_args ?? Array.Empty<string>(), _invocationContext.Console);
        }

        private IConfiguration Configure()
            => Configure(new ConfigurationBuilder()).Build();

        private IConfigurationBuilder Configure(IConfigurationBuilder builder)
        {
            builder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true);
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLowerInvariant() == "development")
            {
                builder.AddUserSecrets<Program>();
            }
            builder
                .AddEnvironmentVariables()
                .AddEnvironmentVariables("ASPNETCORE_")
                .AddEnvironmentVariables("NETCORE_")
                .AddEnvironmentVariables("DOTNET_")
                .AddCommandLine(_args);
            return builder;
        }

        private IServiceCollection ConfigureServices(IServiceCollection services)
            => services
                .AddOptions()
                .AddSingleton(_invocationContext)
                .AddSingleton(p => new ConsoleRenderer(
                        _invocationContext.Console,
                        OutputMode.Ansi,
                        true))
                .Configure<SqlBackupSettings>(Configuration);

        private IServiceProvider ConfigureServices()
            => ConfigureServices(new ServiceCollection())
                .BuildServiceProvider();
    }
}