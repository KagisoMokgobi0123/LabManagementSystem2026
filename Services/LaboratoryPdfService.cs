
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
        // Generates a single-test report. The caller must confirm that the
        // requested test has reached Completed and is not deleted.
        public byte[] GeneratePreliminaryReport(
            int testRequestId,
            int testRequestTestTypeId,
            string patientName,
            string doctorName,
            TestResultModel result)
        {
            if (result == null ||
                result.TestRequestTestTypeId != testRequestTestTypeId ||
                string.IsNullOrWhiteSpace(result.ResultValue))
            {
                throw new InvalidOperationException(
                    "A valid saved result for this individual test is required.");
            }

            var generatedAtUtc = DateTime.UtcNow;
            var referenceRange = result.ReferenceMin.HasValue &&
                                 result.ReferenceMax.HasValue
                ? $"{result.ReferenceMin} - {result.ReferenceMax}"
                : "Not available";

            var resultFlag = !string.IsNullOrWhiteSpace(result.ResultFlag)
                ? result.ResultFlag
                : result.IsAbnormal ? "Abnormal" : "Not evaluated";

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(35);
                    page.DefaultTextStyle(x =>
                        x.FontSize(10).FontColor(Colors.Grey.Darken3));

                    page.Header().Column(column =>
                    {
                        column.Spacing(5);
                        column.Item()
                            .Text("NMB HEALTH LABORATORY SYSTEM")
                            .Bold().FontSize(18)
                            .FontColor(Colors.Blue.Darken3);

                        column.Item()
                            .Text("PRELIMINARY LABORATORY REPORT")
                            .Bold().FontSize(14)
                            .FontColor(Colors.Red.Darken2);

                        column.Item()
                            .Text("NOT INDEPENDENTLY VERIFIED")
                            .Bold().FontSize(11)
                            .FontColor(Colors.Red.Darken2);

                        column.Item().PaddingTop(8)
                            .LineHorizontal(1)
                            .LineColor(Colors.Grey.Lighten1);
                    });

                    page.Content().PaddingVertical(18).Column(column =>
                    {
                        column.Spacing(12);

                        column.Item()
                            .Background(Colors.Grey.Lighten4)
                            .Padding(12)
                            .Column(info =>
                            {
                                info.Spacing(5);
                                info.Item().Text($"Request ID: {testRequestId}");
                                info.Item().Text($"Individual Test ID: {testRequestTestTypeId}");
                                info.Item().Text($"Patient: {patientName}");
                                info.Item().Text($"Requesting Doctor: {doctorName}");
                                info.Item().Text($"Generated: {generatedAtUtc:dd MMM yyyy HH:mm} UTC");
                            });

                        column.Item().Text("INDIVIDUAL TEST RESULT")
                            .Bold().FontSize(12)
                            .FontColor(Colors.Blue.Darken3);

                        column.Item().Text($"Test: {result.ResultName ?? "Laboratory Result"}");
                        column.Item().Text($"Result: {result.ResultValue} {result.MeasurementUnit ?? string.Empty}");
                        column.Item().Text($"Reference range: {referenceRange}");
                        column.Item().Text($"Flag: {resultFlag}");
                        column.Item().Text($"Captured: {result.CapturedDateTime:dd MMM yyyy HH:mm} UTC");

                        if (!string.IsNullOrWhiteSpace(result.Notes))
                        {
                            column.Item().Text($"Notes: {result.Notes}");
                        }

                        column.Item().PaddingTop(12)
                            .Text("PRELIMINARY RESULT — NOT INDEPENDENTLY VERIFIED. " +
                                  "This result is subject to review and correction. " +
                                  "It must not be represented as a final verified report.")
                            .Bold().FontColor(Colors.Red.Darken2);

                        column.Item().Text(
                            "Confidential medical information for the requesting healthcare professional only.")
                            .Italic().FontSize(8);
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("NMB-HLabSys | Preliminary | Page ");
                        text.CurrentPageNumber();
                        text.Span(" of ");
                        text.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }

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
        // Portal-only partial report. Only independently verified saved results
        // should be supplied; missing test IDs must be disclosed explicitly.
        public byte[] GeneratePartialVerifiedReport(
            int testRequestId,
            string patientName,
            string doctorName,
            List<TestResultModel> verifiedResults,
            List<int> missingTestIds)
        {
            if (verifiedResults == null || verifiedResults.Count == 0)
                throw new InvalidOperationException("At least one verified result is required.");
            if (missingTestIds == null || missingTestIds.Count == 0)
                throw new InvalidOperationException("Use the final report workflow for complete requests.");
            if (verifiedResults.Any(r => string.IsNullOrWhiteSpace(r.ResultValue) ||
                                         string.IsNullOrWhiteSpace(r.VerifiedByTechnicianId) ||
                                         !r.VerifiedDateTime.HasValue))
                throw new InvalidOperationException("Unverified results cannot appear in this report.");

            var generated = DateTime.UtcNow;
            return Document.Create(doc =>
            {
                doc.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(35);
                    page.DefaultTextStyle(s => s.FontSize(10));
                    page.Header().Column(h =>
                    {
                        h.Item().Text("NMB HEALTH LABORATORY SYSTEM").Bold().FontSize(17)
                            .FontColor(Colors.Blue.Darken3);
                        h.Item().Text("PARTIAL VERIFIED RESULTS — INCOMPLETE REQUEST RECORD")
                            .Bold().FontSize(12).FontColor(Colors.Red.Darken2);
                        h.Item().Text("NOT A COMPLETE FINAL LABORATORY REPORT")
                            .Bold().FontColor(Colors.Red.Darken2);
                        h.Item().PaddingTop(8).LineHorizontal(1);
                    });
                    page.Content().PaddingVertical(16).Column(c =>
                    {
                        c.Spacing(10);
                        c.Item().Text($"Request: #{testRequestId}");
                        c.Item().Text($"Patient: {patientName}");
                        c.Item().Text($"Requesting doctor: {doctorName}");
                        c.Item().Text($"Generated (UTC): {generated:dd MMM yyyy HH:mm}");
                        c.Item().Text($"Recorded verified results: {verifiedResults.Count}").Bold();
                        foreach (var r in verifiedResults)
                        {
                            c.Item().Background(Colors.Grey.Lighten4).Padding(9).Column(x =>
                            {
                                x.Item().Text($"Test ID: {r.TestRequestTestTypeId} — {r.ResultName ?? "Laboratory result"}").Bold();
                                x.Item().Text($"Result: {r.ResultValue} {r.MeasurementUnit ?? ""}");
                                x.Item().Text($"Flag: {r.ResultFlag ?? (r.IsAbnormal ? "Abnormal" : "Not evaluated")}");
                                x.Item().Text($"Verified by: {r.VerifiedByTechnician?.FullName ?? r.VerifiedByTechnicianId}");
                                x.Item().Text($"Verified (UTC): {r.VerifiedDateTime:dd MMM yyyy HH:mm}");
                                if (!string.IsNullOrWhiteSpace(r.Notes))
                                    x.Item().Text($"Notes: {r.Notes}");
                            });
                        }
                        c.Item().PaddingTop(10).Text("TESTS WITHOUT RECORDED VERIFIED RESULTS")
                            .Bold().FontColor(Colors.Red.Darken2);
                        foreach (var id in missingTestIds.OrderBy(x => x))
                            c.Item().Text($"Individual test ID {id}: no valid independently verified result record available");
                        c.Item().PaddingTop(8).Text(
                            "This document reports only the recorded independently verified results shown above. " +
                            "Other tests in the request lack valid result records. This is NOT a complete final report.")
                            .Bold().FontColor(Colors.Red.Darken2);
                    });
                    page.Footer().AlignCenter().Text("NMB-HLabSys | Confidential | Partial verified results");
                });
            }).GeneratePdf();
        }

    }
}
