using System;
using System.IO;
using System.Linq;
using Contoso.LegacyBank.Documents.Models;
using Newtonsoft.Json;

namespace Contoso.LegacyBank.Documents.Processing
{
    public sealed class JobParser
    {
        public StatementJob ParseFile(string path)
        {
            return Parse(File.ReadAllText(path));
        }

        public StatementJob Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) throw new InvalidDataException("The job document is empty.");

            StatementJob job;
            try
            {
                job = JsonConvert.DeserializeObject<StatementJob>(json, new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Error,
                    DateParseHandling = DateParseHandling.DateTime
                });
            }
            catch (JsonException exception)
            {
                throw new InvalidDataException("The job JSON does not match the statement contract.", exception);
            }

            if (job == null) throw new InvalidDataException("The job document is empty.");
            if (job.SchemaVersion != 1) throw new InvalidDataException("Only schemaVersion 1 is supported.");
            if (string.IsNullOrWhiteSpace(job.JobId) || job.JobId.Length > 100 || job.JobId.Any(c => !char.IsLetterOrDigit(c) && c != '-' && c != '_'))
                throw new InvalidDataException("jobId must contain only letters, digits, hyphens, or underscores.");
            if (string.IsNullOrWhiteSpace(job.Customer)) throw new InvalidDataException("customer is required.");
            if (string.IsNullOrWhiteSpace(job.Account)) throw new InvalidDataException("account is required.");
            if (job.FromDate.Date > job.ToDate.Date) throw new InvalidDataException("fromDate must not be after toDate.");
            if (job.Transactions == null) throw new InvalidDataException("transactions is required.");
            if (job.Transactions.Any(t => t == null || string.IsNullOrWhiteSpace(t.Description)))
                throw new InvalidDataException("Every transaction requires a description.");
            return job;
        }
    }
}
