using System.IO;
using Contoso.LegacyBank.Documents.Processing;
using NUnit.Framework;

namespace Contoso.LegacyBank.Documents.Tests
{
    [TestFixture]
    public sealed class JobParserTests
    {
        [Test]
        public void Parse_ValidVersionOneJob_ReturnsTypedJob()
        {
            var job = new JobParser().Parse(TestData.ValidJob);

            Assert.That(job.JobId, Is.EqualTo("statement-1001"));
            Assert.That(job.Transactions, Has.Count.EqualTo(2));
            Assert.That(job.ClosingBalance, Is.EqualTo(1275.50m));
        }

        [Test]
        public void Parse_UnsupportedVersion_ThrowsUsefulValidationError()
        {
            var json = TestData.ValidJob.Replace(@"""schemaVersion"": 1", @"""schemaVersion"": 2");

            var exception = Assert.Throws<InvalidDataException>(() => new JobParser().Parse(json));

            Assert.That(exception.Message, Does.Contain("schemaVersion 1"));
        }

        [Test]
        public void Parse_UnknownProperty_RejectsContractDrift()
        {
            var json = TestData.ValidJob.Replace(@"""jobId"":", @"""unexpected"": true, ""jobId"":");

            Assert.Throws<InvalidDataException>(() => new JobParser().Parse(json));
        }
    }
}
