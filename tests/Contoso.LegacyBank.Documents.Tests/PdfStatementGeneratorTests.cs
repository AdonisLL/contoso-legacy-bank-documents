using System;
using System.IO;
using Contoso.LegacyBank.Documents.Processing;
using NUnit.Framework;
using PdfSharp.Pdf.IO;

namespace Contoso.LegacyBank.Documents.Tests
{
    [TestFixture]
    public sealed class PdfStatementGeneratorTests
    {
        [Test]
        public void Generate_ValidStatement_WritesReadableBrandedPdf()
        {
            var directory = Path.Combine(TestContext.CurrentContext.WorkDirectory, "pdf-" + Guid.NewGuid().ToString("N"));
            var path = Path.Combine(directory, "statement.pdf");
            try
            {
                var job = new JobParser().Parse(TestData.ValidJob);
                new PdfStatementGenerator().Generate(job, path);

                Assert.That(File.Exists(path), Is.True);
                Assert.That(new FileInfo(path).Length, Is.GreaterThan(1000));
                using (var document = PdfReader.Open(path, PdfDocumentOpenMode.ReadOnly))
                {
                    Assert.That(document.PageCount, Is.EqualTo(1));
                    Assert.That(document.Info.Title, Does.Contain("Contoso"));
                }
            }
            finally
            {
                if (Directory.Exists(directory)) Directory.Delete(directory, true);
            }
        }
    }
}
