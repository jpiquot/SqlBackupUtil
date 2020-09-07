using System;
using System.CommandLine.Invocation;

namespace SqlBackupUtil
{
    internal interface ICommandHandlerBuilder
    {
        ICommandHandlerBuilder AddOptions(CommandOptions services);

        ICommandHandlerBuilder AddServiceProvider(IServiceProvider services);

        ICommandHandler Build();
    }
}