using System;
using System.IO;
using System.Linq;
using Contoso.LegacyBank.Documents.Configuration;
using Contoso.LegacyBank.Documents.Processing;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Contoso.LegacyBank.Documents.Tests
{
    [TestFixture]
    public sealed class DocumentWorkerTests
    {
        [Test]
        public void ProcessAvailable_MalformedJob_PreservesInputAndWritesSafeFailureMetadata()
        {
            var root = Path.Combine(TestContext.CurrentContext.WorkDirectory, "failure-" + Guid.NewGuid().ToString("N"));
            var configuration = WorkerConfiguration.Create(root, TimeSpan.FromMilliseconds(10));
            configuration.EnsureDirectories();
            File.WriteAllText(Path.Combine(configuration.Pending, "broken.json"), @"{ ""schemaVersion"": 1, ""secret"": ""do-not-copy"", ""jobId"": ");

            try
            {
                var count = new DocumentWorker(configuration).ProcessAvailable();

                Assert.That(count, Is.EqualTo(1));
                Assert.That(Directory.GetFiles(configuration.Processing), Is.Empty);
                Assert.That(Directory.GetFiles(configuration.Failed, "*.job.json"), Has.Length.EqualTo(1));
                var metadataPath = Directory.GetFiles(configuration.Failed, "*.failure.json").Single();
                var metadataText = File.ReadAllText(metadataPath);
                var metadata = JObject.Parse(metadataText);
                Assert.That((string)metadata["status"], Is.EqualTo("failed"));
                Assert.That((string)metadata["diagnosticId"], Is.Not.Empty);
                Assert.That(metadataText, Does.Not.Contain("do-not-copy"));
            }
            finally
            {
                if (Directory.Exists(root)) Directory.Delete(root, true);
            }
        }

        [Test]
        public void ProcessAvailable_ValidJob_WritesPdfAndCompletionMetadata()
        {
            var root = Path.Combine(TestContext.CurrentContext.WorkDirectory, "success-" + Guid.NewGuid().ToString("N"));
            var configuration = WorkerConfiguration.Create(root, TimeSpan.FromMilliseconds(10));
            configuration.EnsureDirectories();
            File.WriteAllText(Path.Combine(configuration.Pending, "statement.json"), TestData.ValidJob);

            try
            {
                new DocumentWorker(configuration).ProcessAvailable();

                Assert.That(File.Exists(Path.Combine(configuration.Output, "statement-1001.pdf")), Is.True);
                var metadata = JObject.Parse(File.ReadAllText(Path.Combine(configuration.Completed, "statement-1001.completion.json")));
                Assert.That((string)metadata["status"], Is.EqualTo("completed"));
                Assert.That((string)metadata["pdfPath"], Does.EndWith("statement-1001.pdf"));
            }
            finally
            {
                if (Directory.Exists(root)) Directory.Delete(root, true);
            }
        }
    }
}
