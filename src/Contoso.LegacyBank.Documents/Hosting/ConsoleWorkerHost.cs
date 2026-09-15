using System;
using System.Threading;
using Contoso.LegacyBank.Documents.Configuration;
using Contoso.LegacyBank.Documents.Processing;

namespace Contoso.LegacyBank.Documents.Hosting
{
    public sealed class ConsoleWorkerHost
    {
        private readonly DocumentWorker worker;
        private readonly TimeSpan pollingInterval;

        public ConsoleWorkerHost(WorkerConfiguration configuration)
        {
            worker = new DocumentWorker(configuration);
            pollingInterval = configuration.PollingInterval;
        }

        public int RunOnce()
        {
            return worker.ProcessAvailable();
        }

        public void Run(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                worker.ProcessAvailable();
                cancellationToken.WaitHandle.WaitOne(pollingInterval);
            }
        }
    }
}
