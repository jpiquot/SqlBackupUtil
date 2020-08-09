using System.IO;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NLog.Web;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using System;
using System.CommandLine.Invocation;
using System.CommandLine.Rendering;

namespace SqlBackupUtil
{
    internal class Program
    {
        public static Task<IHostBuilder> CreateHostBuilder(IHostBuilder builder, string[]? args)
        {
            return Task.FromResult(builder
                        .UseConsoleLifetime()
                        .UseNLog()
                        .ConfigureHostConfiguration((config) =>
                        {
                            config
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddEnvironmentVariables()
                                .AddEnvironmentVariables("ASPNETCORE_")
                                .AddEnvironmentVariables("NETCORE_")
                                .AddEnvironmentVariables("DOTNET_")
                                .AddCommandLine(args);
                        })
                        .ConfigureServices((hostContext, services) =>
                        {
                            // services.AddTransient<IDynamicsAXRepository, DynamicsAxRepository>();
                            // services.AddHostedService<RestoreWorker>();
                            services.AddOptions();
                            // services.AddOptions<ActiveDirectorySettings>().Configure<IConfiguration>((o,
                            // c) => o.Update(c));
                        })
                        .ConfigureAppConfiguration((context, builder) =>
                        {
                            builder
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: true)
                                .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
                                .AddEnvironmentVariables()
                                .AddEnvironmentVariables("ASPNETCORE_")
                                .AddEnvironmentVariables("NETCORE_")
                                .AddEnvironmentVariables("DOTNET_")
                                .AddUserSecrets<Program>()
                                .AddCommandLine(args);
                            if (context.HostingEnvironment.IsDevelopment())
                            {
                                builder.AddUserSecrets<Program>();
                            }
                        }));
        }

        public static async Task Main(InvocationContext invocationContext, string[] args)
        {
            var consoleRenderer = new ConsoleRenderer(
                invocationContext.Console,
                OutputMode.Ansi,
                true);
            if (invocationContext.Console is ITerminal terminal)
            {
                terminal.Clear();
            }
            string[] arguments = args ?? Array.Empty<string>();
            await Commands
               .CreateBuilder(invocationContext, consoleRenderer)
               .UseHost(_ => Host.CreateDefaultBuilder(), host => CreateHostBuilder(host, arguments))
               .UseDefaults()
               .Build()
               .InvokeAsync(arguments);
        }
    }
}