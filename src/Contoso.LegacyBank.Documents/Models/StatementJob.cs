using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Contoso.LegacyBank.Documents.Models
{
    public sealed class StatementJob
    {
        [JsonProperty("schemaVersion", Required = Required.Always)]
        public int SchemaVersion { get; set; }

        [JsonProperty("jobId", Required = Required.Always)]
        public string JobId { get; set; }

        [JsonProperty("customer", Required = Required.Always)]
        public string Customer { get; set; }

        [JsonProperty("account", Required = Required.Always)]
        public string Account { get; set; }

        [JsonProperty("fromDate", Required = Required.Always)]
        public DateTime FromDate { get; set; }

        [JsonProperty("toDate", Required = Required.Always)]
        public DateTime ToDate { get; set; }

        [JsonProperty("openingBalance", Required = Required.Always)]
        public decimal OpeningBalance { get; set; }

        [JsonProperty("closingBalance", Required = Required.Always)]
        public decimal ClosingBalance { get; set; }

        [JsonProperty("transactions", Required = Required.Always)]
        public IList<StatementTransaction> Transactions { get; set; }
    }

    public sealed class StatementTransaction
    {
        [JsonProperty("date", Required = Required.Always)]
        public DateTime Date { get; set; }

        [JsonProperty("description", Required = Required.Always)]
        public string Description { get; set; }

        [JsonProperty("amount", Required = Required.Always)]
        public decimal Amount { get; set; }

        [JsonProperty("balance", Required = Required.Always)]
        public decimal Balance { get; set; }
    }
}
