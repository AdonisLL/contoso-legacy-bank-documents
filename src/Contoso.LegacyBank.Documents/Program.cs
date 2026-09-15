using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using Contoso.LegacyBank.Documents.Configuration;
using Contoso.LegacyBank.Documents.Hosting;

namespace Contoso.LegacyBank.Documents
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            if (args.Any(arg => string.Equals(arg, "--service", StringComparison.OrdinalIgnoreCase)))
            {
                ServiceBase.Run(new DocumentWindowsService());
                return 0;
            }

            var configuration = WorkerConfiguration.Load();
            var host = new ConsoleWorkerHost(configuration);
            if (args.Any(arg => string.Equals(arg, "--once", StringComparison.OrdinalIgnoreCase)))
            {
                var count = host.RunOnce();
                Console.WriteLine("Processed {0} job(s).", count);
                return 0;
            }

            using (var cancellation = new CancellationTokenSource())
            {
                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    eventArgs.Cancel = true;
                    cancellation.Cancel();
                };
                Console.WriteLine("Watching {0}. Press Ctrl+C to stop.", configuration.Pending);
                host.Run(cancellation.Token);
            }
            return 0;
        }
    }
}
