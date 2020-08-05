using System.IO;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NLog.Web;

namespace RestoreDatabase
{
    internal class Program
    {
        public static Task<IHostBuilder> CreateHostBuilder(string[] args)
        {
            return Task.FromResult(new HostBuilder()
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
                            services.AddHostedService<RestoreWorker>();
                            services.AddOptions();
                            // services.AddOptions<ActiveDirectorySettings>().Configure<IConfiguration>((o,
                            // c) => o.Update(c));
                        })
                        .ConfigureAppConfiguration((context, builder) =>
                        {
                            builder
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: false)
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

        public static async Task Main(string[] args)
                    => await (await CreateHostBuilder(args))
                .RunConsoleAsync();
    }
}