using System;
using System.CommandLine.Invocation;
using System.CommandLine.Rendering;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SqlBackupUtil
{
    internal abstract class CommandHandler<TH, TO> : ICommandHandler
        where TH : ICommandHandler
        where TO : CommandOptions
    {
        public CommandHandler(IServiceProvider serviceProvider, TO options)
        {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            ConsoleRenderer = serviceProvider.GetRequiredService<ConsoleRenderer>();
            Logger = serviceProvider.GetRequiredService<ILogger<TH>>();
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        protected ConsoleRenderer ConsoleRenderer { get; }
        protected ILogger<TH> Logger { get; }
        protected TO Options { get; }
        protected IServiceProvider ServiceProvider { get; }

        public abstract Task<int> InvokeAsync(InvocationContext context);
    }
}