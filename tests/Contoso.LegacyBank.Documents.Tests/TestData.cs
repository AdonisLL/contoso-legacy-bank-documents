namespace Contoso.LegacyBank.Documents.Tests
{
    internal static class TestData
    {
        public const string ValidJob = @"{
  ""schemaVersion"": 1,
  ""jobId"": ""statement-1001"",
  ""customer"": ""Ada Lovelace"",
  ""account"": ""1234567890"",
  ""fromDate"": ""2026-08-01"",
  ""toDate"": ""2026-08-31"",
  ""openingBalance"": 1200.00,
  ""closingBalance"": 1275.50,
  ""transactions"": [
    { ""date"": ""2026-08-04"", ""description"": ""Payroll deposit"", ""amount"": 500.00, ""balance"": 1700.00 },
    { ""date"": ""2026-08-10"", ""description"": ""Utility payment"", ""amount"": -424.50, ""balance"": 1275.50 }
  ]
}";
    }
}
