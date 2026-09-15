using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Contoso.LegacyBank.Documents.Configuration;
using Contoso.LegacyBank.Documents.Models;
using Newtonsoft.Json;

namespace Contoso.LegacyBank.Documents.Processing
{
    public sealed class DocumentWorker
    {
        private readonly WorkerConfiguration configuration;
        private readonly JobParser parser;
        private readonly PdfStatementGenerator pdfGenerator;

        public DocumentWorker(WorkerConfiguration configuration)
            : this(configuration, new JobParser(), new PdfStatementGenerator())
        {
        }

        public DocumentWorker(WorkerConfiguration configuration, JobParser parser, PdfStatementGenerator pdfGenerator)
        {
            this.configuration = configuration;
            this.parser = parser;
            this.pdfGenerator = pdfGenerator;
        }

        public int ProcessAvailable()
        {
            configuration.EnsureDirectories();
            var processed = 0;
            foreach (var pendingPath in Directory.GetFiles(configuration.Pending, "*.json").OrderBy(path => path, StringComparer.OrdinalIgnoreCase))
            {
                var processingPath = Path.Combine(configuration.Processing, Path.GetFileName(pendingPath));
                try
                {
                    File.Move(pendingPath, processingPath);
                }
                catch (IOException)
                {
                    continue;
                }

                ProcessClaimed(processingPath);
                processed++;
            }
            return processed;
        }

        private void ProcessClaimed(string processingPath)
        {
            StatementJob job = null;
            try
            {
                job = parser.ParseFile(processingPath);
                var pdfPath = Path.Combine(configuration.Output, job.JobId + ".pdf");
                var temporaryPdfPath = pdfPath + "." + Guid.NewGuid().ToString("N") + ".tmp";
                pdfGenerator.Generate(job, temporaryPdfPath);
                ReplaceAtomically(temporaryPdfPath, pdfPath);

                WriteJsonAtomically(
                    Path.Combine(configuration.Completed, job.JobId + ".completion.json"),
                    new CompletionMetadata
                    {
                        SchemaVersion = 1,
                        JobId = job.JobId,
                        Status = "completed",
                        PdfPath = pdfPath,
                        CompletedUtc = DateTime.UtcNow
                    });
                File.Delete(processingPath);
                Console.WriteLine("Completed job {0}.", job.JobId);
            }
            catch (Exception exception)
            {
                RecordFailure(processingPath, job == null ? Path.GetFileNameWithoutExtension(processingPath) : job.JobId, exception);
            }
        }

        private void RecordFailure(string processingPath, string jobId, Exception exception)
        {
            var diagnosticId = Guid.NewGuid().ToString("N");
            var safeId = SanitizeFileName(string.IsNullOrWhiteSpace(jobId) ? "unknown-job" : jobId);
            var preservedJobPath = Path.Combine(configuration.Failed, safeId + "." + diagnosticId + ".job.json");
            try
            {
                if (File.Exists(processingPath)) File.Move(processingPath, preservedJobPath);
            }
            catch (IOException)
            {
                preservedJobPath = processingPath;
            }

            var metadataPath = Path.Combine(configuration.Failed, safeId + "." + diagnosticId + ".failure.json");
            WriteJsonAtomically(metadataPath, new FailureMetadata
            {
                SchemaVersion = 1,
                JobId = safeId,
                Status = "failed",
                FailedUtc = DateTime.UtcNow,
                DiagnosticId = diagnosticId,
                ErrorType = exception.GetType().Name,
                Message = SafeMessage(exception),
                PreservedJobPath = preservedJobPath
            });
            Console.Error.WriteLine("Job {0} failed. Diagnostic ID: {1}.", safeId, diagnosticId);
        }

        private static string SafeMessage(Exception exception)
        {
            var allowed = new HashSet<string>(StringComparer.Ordinal)
            {
                "InvalidDataException", "JsonSerializationException", "JsonReaderException",
                "IOException", "UnauthorizedAccessException", "ArgumentException"
            };
            if (!allowed.Contains(exception.GetType().Name)) return "The document could not be generated.";
            var message = exception.Message ?? "The document could not be generated.";
            message = message.Replace(Environment.NewLine, " ").Replace("\r", " ").Replace("\n", " ");
            return message.Length <= 500 ? message : message.Substring(0, 500);
        }

        private static string SanitizeFileName(string value)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var safe = new string(value.Select(c => invalid.Contains(c) ? '_' : c).ToArray());
            return safe.Length > 100 ? safe.Substring(0, 100) : safe;
        }

        private static void WriteJsonAtomically(string path, object value)
        {
            var temporaryPath = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            File.WriteAllText(temporaryPath, JsonConvert.SerializeObject(value, Formatting.Indented));
            ReplaceAtomically(temporaryPath, path);
        }

        private static void ReplaceAtomically(string temporaryPath, string finalPath)
        {
            if (File.Exists(finalPath))
            {
                var backupPath = finalPath + ".bak";
                try
                {
                    File.Replace(temporaryPath, finalPath, backupPath);
                }
                finally
                {
                    if (File.Exists(backupPath)) File.Delete(backupPath);
                }
            }
            else
            {
                File.Move(temporaryPath, finalPath);
            }
        }
    }
}
