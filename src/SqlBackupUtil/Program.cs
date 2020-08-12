using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace SqlBackupUtil
{
    internal class Program
    {
        public static Task<int> Main(InvocationContext invocationContext, string[]? args)
            => new Startup(invocationContext, args)
                .StartAsync();
    }
}