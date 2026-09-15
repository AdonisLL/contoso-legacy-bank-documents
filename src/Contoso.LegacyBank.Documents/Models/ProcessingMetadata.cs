using System;
using Newtonsoft.Json;

namespace Contoso.LegacyBank.Documents.Models
{
    public sealed class CompletionMetadata
    {
        [JsonProperty("schemaVersion")]
        public int SchemaVersion { get; set; }
        [JsonProperty("jobId")]
        public string JobId { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("pdfPath")]
        public string PdfPath { get; set; }
        [JsonProperty("completedUtc")]
        public DateTime CompletedUtc { get; set; }
    }

    public sealed class FailureMetadata
    {
        [JsonProperty("schemaVersion")]
        public int SchemaVersion { get; set; }
        [JsonProperty("jobId")]
        public string JobId { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("failedUtc")]
        public DateTime FailedUtc { get; set; }
        [JsonProperty("diagnosticId")]
        public string DiagnosticId { get; set; }
        [JsonProperty("errorType")]
        public string ErrorType { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("preservedJobPath")]
        public string PreservedJobPath { get; set; }
    }
}
