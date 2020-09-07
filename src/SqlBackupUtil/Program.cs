using System;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

using NLog;
using NLog.Web;

namespace SqlBackupUtil
{
    internal class Program
    {
        public static async Task<int> Main(InvocationContext invocationContext, string[]? args)
        {
            var logger = LogManager.Setup()
                           .RegisterNLogWeb()
                           .LoadConfigurationFromFile("nlog.config")
                           .GetCurrentClassLogger();
            try
            {
                return await new Startup(invocationContext, args)
                    .StartAsync();
            }
            catch (Exception exception)
            {
                //NLog: catch setup errors
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid
                // segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }
    }
}