
using System;
using System.Collections.Generic;
using System.Linq;
using NMB_HLabSys.Areas.Technician.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace NMB_HLabSys.Services
{
    public class LaboratoryPdfService
    {
        public byte[] GenerateLaboratoryReport(
            int testRequestId,
            string patientName,
            string doctorName,
            List<TestResultModel> results)
        {
            if (results == null || results.Count == 0)
            {
                throw new InvalidOperationException(
                    "Cannot generate a laboratory report without results.");
            }

            var reportDate = DateTime.Now;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(35);
                    page.DefaultTextStyle(x =>
                        x.FontSize(10).FontColor(Colors.Grey.Darken3));

                    // ================================
                    // HEADER
                    // ================================
                    page.Header().Column(column =>
                    {
                        column.Item().Text(
                            "NMB HEALTH LABORATORY SYSTEM")
                            .Bold()
                            .FontSize(19)
                            .FontColor(Colors.Blue.Darken3);

                        column.Item().PaddingTop(4).Text(
                            "VERIFIED LABORATORY RESULTS REPORT")
                            .SemiBold()
                            .FontSize(12)
                            .FontColor(Colors.Blue.Medium);

                        column.Item()
                            .PaddingTop(10)
                            .LineHorizontal(1)
                            .LineColor(Colors.Grey.Lighten1);
                    });

                    // ================================
                    // MAIN CONTENT
                    // ================================
                    page.Content().PaddingVertical(20).Column(column =>
                    {
                        column.Spacing(15);

                        // PATIENT AND REQUEST DETAILS
                        column.Item()
                            .Background(Colors.Grey.Lighten4)
                            .Padding(14)
                            .Column(info =>
                            {
                                info.Spacing(6);

                                info.Item().Text("REPORT INFORMATION")
                                    .Bold()
                                    .FontSize(11);

                                info.Item().Text(
                                    $"Request ID: {testRequestId}");

                                info.Item().Text(
                                    $"Patient: {patientName}");

                                info.Item().Text(
                                    $"Requesting Doctor: {doctorName}");

                                info.Item().Text(
                                    $"Report Generated: {reportDate:dd MMM yyyy HH:mm}");
                            });

                        // TEST RESULTS
                        column.Item().Text("LABORATORY TEST RESULTS")
                            .Bold()
                            .FontSize(13)
                            .FontColor(Colors.Blue.Darken3);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2.2f);
                                columns.RelativeColumn(1.4f);
                                columns.RelativeColumn(1.1f);
                                columns.RelativeColumn(1.6f);
                                columns.RelativeColumn(1.1f);
                            });

                            table.Header(header =>
                            {
                                void HeaderCell(string value)
                                {
                                    header.Cell()
                                        .Background(Colors.Blue.Darken3)
                                        .Padding(7)
                                        .Text(value)
                                        .Bold()
                                        .FontSize(8)
                                        .FontColor(Colors.White);
                                }

                                HeaderCell("Test / Result");
                                HeaderCell("Value");
                                HeaderCell("Unit");
                                HeaderCell("Reference");
                                HeaderCell("Flag");
                            });

                            foreach (var result in results)
                            {
                                var reference =
                                    result.ReferenceMin.HasValue &&
                                    result.ReferenceMax.HasValue
                                        ? $"{result.ReferenceMin} - {result.ReferenceMax}"
                                        : "N/A";

                                var flag = !string.IsNullOrWhiteSpace(
                                    result.ResultFlag)
                                        ? result.ResultFlag
                                        : result.IsAbnormal
                                            ? "Abnormal"
                                            : "Normal";

                                var background = result.IsAbnormal
                                    ? Colors.Red.Lighten5
                                    : Colors.White;

                                void ResultCell(string value)
                                {
                                    table.Cell()
                                        .Background(background)
                                        .BorderBottom(0.5f)
                                        .BorderColor(Colors.Grey.Lighten2)
                                        .Padding(7)
                                        .Text(value)
                                        .FontSize(8);
                                }

                                ResultCell(
                                    result.ResultName ?? "Laboratory Result");

                                ResultCell(
                                    result.ResultValue ?? "N/A");

                                ResultCell(
                                    result.MeasurementUnit ?? "-");

                                ResultCell(reference);

                                ResultCell(flag);
                            }
                        });

                        // NOTES
                        column.Item().Text("RESULT NOTES")
                            .Bold()
                            .FontSize(11)
                            .FontColor(Colors.Blue.Darken3);

                        foreach (var result in results)
                        {
                            if (!string.IsNullOrWhiteSpace(result.Notes))
                            {
                                column.Item().Text(
                                    $"{result.ResultName}: {result.Notes}")
                                    .FontSize(9);
                            }
                        }

                        // VERIFICATION DETAILS
                        column.Item().Text("VERIFICATION DETAILS")
                            .Bold()
                            .FontSize(11)
                            .FontColor(Colors.Blue.Darken3);

                        foreach (var result in results)
                        {
                            var verifier =
                                result.VerifiedByTechnician?.FullName
                                ?? result.VerifiedByTechnicianId
                                ?? "Unknown";

                            var verifiedDate =
                                result.VerifiedDateTime.HasValue
                                    ? result.VerifiedDateTime.Value
                                        .ToString("dd MMM yyyy HH:mm")
                                    : "Not recorded";

                            column.Item().Text(
                                $"{result.ResultName} — " +
                                $"Verified by: {verifier} | " +
                                $"Date: {verifiedDate}")
                                .FontSize(9);
                        }

                        // DISCLAIMER
                        column.Item()
                            .PaddingTop(15)
                            .LineHorizontal(1)
                            .LineColor(Colors.Grey.Lighten2);

                        column.Item().Text(
                            "This report contains confidential medical " +
                            "information intended only for the authorized " +
                            "requesting healthcare professional.")
                            .Italic()
                            .FontSize(8)
                            .FontColor(Colors.Grey.Darken1);
                    });

                    // ================================
                    // FOOTER
                    // ================================
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span(
                            "NMB Health Laboratory System | Page ");

                        text.CurrentPageNumber();

                        text.Span(" of ");

                        text.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
