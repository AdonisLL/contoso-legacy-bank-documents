using System;
using System.Globalization;
using System.IO;
using Contoso.LegacyBank.Documents.Models;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Contoso.LegacyBank.Documents.Processing
{
    public sealed class PdfStatementGenerator
    {
        private static readonly XColor Navy = XColor.FromArgb(17, 55, 92);
        private static readonly XColor Gold = XColor.FromArgb(215, 164, 52);

        public void Generate(StatementJob job, string outputPath)
        {
            var document = new PdfDocument();
            document.Info.Title = "Contoso Legacy Bank Statement";
            document.Info.Subject = "Account statement";
            document.Info.Author = "Contoso Legacy Bank";

            var page = document.AddPage();
            var graphics = XGraphics.FromPdfPage(page);
            var titleFont = new XFont("Arial", 20, XFontStyle.Bold);
            var headingFont = new XFont("Arial", 10, XFontStyle.Bold);
            var bodyFont = new XFont("Arial", 9, XFontStyle.Regular);
            var smallFont = new XFont("Arial", 7, XFontStyle.Regular);
            double y = 0;

            DrawHeader(graphics, page, titleFont, smallFont);
            y = 95;
            graphics.DrawString("STATEMENT", headingFont, new XSolidBrush(Navy), 40, y);
            y += 22;
            DrawLabelValue(graphics, "Customer", job.Customer, 40, y, headingFont, bodyFont);
            DrawLabelValue(graphics, "Account", MaskAccount(job.Account), 320, y, headingFont, bodyFont);
            y += 18;
            DrawLabelValue(graphics, "Period", job.FromDate.ToString("dd MMM yyyy", CultureInfo.InvariantCulture) + " - " + job.ToDate.ToString("dd MMM yyyy", CultureInfo.InvariantCulture), 40, y, headingFont, bodyFont);
            y += 26;

            graphics.DrawRectangle(new XSolidBrush(Navy), 40, y, 515, 22);
            DrawCell(graphics, "Date", 45, y + 15, headingFont, XBrushes.White);
            DrawCell(graphics, "Description", 115, y + 15, headingFont, XBrushes.White);
            DrawCell(graphics, "Amount", 405, y + 15, headingFont, XBrushes.White);
            DrawCell(graphics, "Balance", 480, y + 15, headingFont, XBrushes.White);
            y += 38;

            foreach (var transaction in job.Transactions)
            {
                if (y > page.Height.Point - 70)
                {
                    graphics.Dispose();
                    page = document.AddPage();
                    graphics = XGraphics.FromPdfPage(page);
                    DrawHeader(graphics, page, titleFont, smallFont);
                    y = 90;
                }

                graphics.DrawString(transaction.Date.ToString("dd MMM yy", CultureInfo.InvariantCulture), bodyFont, XBrushes.Black, 45, y);
                graphics.DrawString(Truncate(transaction.Description, 48), bodyFont, XBrushes.Black, 115, y);
                DrawRight(graphics, transaction.Amount.ToString("N2", CultureInfo.InvariantCulture), bodyFont, 465, y);
                DrawRight(graphics, transaction.Balance.ToString("N2", CultureInfo.InvariantCulture), bodyFont, 550, y);
                graphics.DrawLine(XPens.LightGray, 40, y + 6, 555, y + 6);
                y += 19;
            }

            y += 8;
            graphics.DrawLine(new XPen(Gold, 2), 330, y, 555, y);
            y += 17;
            DrawLabelValue(graphics, "Opening balance", job.OpeningBalance.ToString("N2", CultureInfo.InvariantCulture), 350, y, headingFont, bodyFont);
            y += 18;
            DrawLabelValue(graphics, "Closing balance", job.ClosingBalance.ToString("N2", CultureInfo.InvariantCulture), 350, y, headingFont, bodyFont);
            graphics.Dispose();

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            document.Save(outputPath);
            document.Close();
        }

        private static void DrawHeader(XGraphics graphics, PdfPage page, XFont titleFont, XFont smallFont)
        {
            graphics.DrawRectangle(new XSolidBrush(Navy), 0, 0, page.Width.Point, 70);
            graphics.DrawRectangle(new XSolidBrush(Gold), 0, 70, page.Width.Point, 5);
            graphics.DrawString("CONTOSO", titleFont, XBrushes.White, 40, 34);
            graphics.DrawString("LEGACY BANK", smallFont, new XSolidBrush(Gold), 42, 52);
            graphics.DrawString("Trusted banking since 1924", smallFont, XBrushes.White, 405, 43);
        }

        private static void DrawLabelValue(XGraphics graphics, string label, string value, double x, double y, XFont labelFont, XFont valueFont)
        {
            graphics.DrawString(label + ":", labelFont, new XSolidBrush(Navy), x, y);
            graphics.DrawString(value, valueFont, XBrushes.Black, x + 95, y);
        }

        private static void DrawCell(XGraphics graphics, string value, double x, double y, XFont font, XBrush brush)
        {
            graphics.DrawString(value, font, brush, x, y);
        }

        private static void DrawRight(XGraphics graphics, string value, XFont font, double right, double y)
        {
            var size = graphics.MeasureString(value, font);
            graphics.DrawString(value, font, XBrushes.Black, right - size.Width, y);
        }

        private static string MaskAccount(string account)
        {
            return account.Length <= 4 ? account : new string('*', account.Length - 4) + account.Substring(account.Length - 4);
        }

        private static string Truncate(string value, int length)
        {
            return value.Length <= length ? value : value.Substring(0, length - 1) + "…";
        }
    }
}
