using System.ServiceProcess;
using System.Threading;
using Contoso.LegacyBank.Documents.Configuration;

namespace Contoso.LegacyBank.Documents.Hosting
{
    public sealed class DocumentWindowsService : ServiceBase
    {
        private CancellationTokenSource cancellation;
        private Thread workerThread;

        public DocumentWindowsService()
        {
            ServiceName = "ContosoLegacyBankDocuments";
            CanStop = true;
            AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            cancellation = new CancellationTokenSource();
            var host = new ConsoleWorkerHost(WorkerConfiguration.Load());
            workerThread = new Thread(() => host.Run(cancellation.Token)) { IsBackground = true, Name = ServiceName + "Worker" };
            workerThread.Start();
        }

        protected override void OnStop()
        {
            if (cancellation == null) return;
            cancellation.Cancel();
            if (workerThread != null) workerThread.Join(System.TimeSpan.FromSeconds(30));
            cancellation.Dispose();
            cancellation = null;
        }
    }
}
