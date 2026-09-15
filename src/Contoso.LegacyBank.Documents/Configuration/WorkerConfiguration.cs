using System;
using System.Configuration;
using System.Globalization;
using System.IO;

namespace Contoso.LegacyBank.Documents.Configuration
{
    public sealed class WorkerConfiguration
    {
        public string Root { get; private set; }
        public string Pending { get { return Path.Combine(Root, "Pending"); } }
        public string Processing { get { return Path.Combine(Root, "Processing"); } }
        public string Completed { get { return Path.Combine(Root, "Completed"); } }
        public string Failed { get { return Path.Combine(Root, "Failed"); } }
        public string Output { get { return Path.Combine(Root, "Output"); } }
        public TimeSpan PollingInterval { get; private set; }

        public static WorkerConfiguration Load()
        {
            var configuredRoot = Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["DocumentsRoot"] ?? string.Empty);
            var root = string.IsNullOrWhiteSpace(configuredRoot)
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ContosoLegacyBank", "Documents")
                : Path.GetFullPath(configuredRoot);

            int seconds;
            if (!int.TryParse(ConfigurationManager.AppSettings["PollingIntervalSeconds"], NumberStyles.Integer, CultureInfo.InvariantCulture, out seconds) || seconds < 1)
            {
                seconds = 5;
            }

            return Create(root, TimeSpan.FromSeconds(seconds));
        }

        public static WorkerConfiguration Create(string root, TimeSpan pollingInterval)
        {
            if (string.IsNullOrWhiteSpace(root)) throw new ArgumentException("A documents root is required.", "root");
            return new WorkerConfiguration { Root = Path.GetFullPath(root), PollingInterval = pollingInterval };
        }

        public void EnsureDirectories()
        {
            Directory.CreateDirectory(Pending);
            Directory.CreateDirectory(Processing);
            Directory.CreateDirectory(Completed);
            Directory.CreateDirectory(Failed);
            Directory.CreateDirectory(Output);
        }
    }
}
