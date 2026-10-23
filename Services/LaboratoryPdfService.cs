
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
        // ==========================================
        // 1. PRELIMINARY INDIVIDUAL TEST REPORT
        // ==========================================

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

            var referenceRange =
                result.ReferenceMin.HasValue &&
                result.ReferenceMax.HasValue
                    ? $"{result.ReferenceMin} - {result.ReferenceMax}"
                    : "Not available";

            var resultFlag =
                !string.IsNullOrWhiteSpace(result.ResultFlag)
                    ? result.ResultFlag
                    : result.IsAbnormal
                        ? "Abnormal"
                        : "Not evaluated";

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(35);

                    page.DefaultTextStyle(x =>
                        x.FontSize(10)
                         .FontColor(Colors.Grey.Darken3));

                    // HEADER
                    page.Header().Column(column =>
                    {
                        column.Spacing(5);

                        column.Item()
                            .Text("NMB HEALTH LABORATORY SYSTEM")
                            .Bold()
                            .FontSize(18)
                            .FontColor(Colors.Blue.Darken3);

                        column.Item()
                            .Text("PRELIMINARY LABORATORY REPORT")
                            .Bold()
                            .FontSize(14)
                            .FontColor(Colors.Red.Darken2);

                        column.Item()
                            .Text("NOT INDEPENDENTLY VERIFIED")
                            .Bold()
                            .FontSize(11)
                            .FontColor(Colors.Red.Darken2);

                        column.Item()
                            .PaddingTop(8)
                            .LineHorizontal(1)
                            .LineColor(Colors.Grey.Lighten1);
                    });

                    // CONTENT
                    page.Content()
                        .PaddingVertical(18)
                        .Column(column =>
                        {
                            column.Spacing(12);

                            column.Item()
                                .Background(Colors.Grey.Lighten4)
                                .Padding(12)
                                .Column(info =>
                                {
                                    info.Spacing(5);

                                    info.Item().Text(
                                        $"Request ID: {testRequestId}");

                                    info.Item().Text(
                                        $"Individual Test ID: {testRequestTestTypeId}");

                                    info.Item().Text(
                                        $"Patient: {patientName}");

                                    info.Item().Text(
                                        $"Requesting Doctor: {doctorName}");

                                    info.Item().Text(
                                        $"Generated: {generatedAtUtc:dd MMM yyyy HH:mm} UTC");
                                });

                            column.Item()
                                .Text("INDIVIDUAL TEST RESULT")
                                .Bold()
                                .FontSize(12)
                                .FontColor(Colors.Blue.Darken3);

                            column.Item().Text(
                                $"Test: {result.ResultName}");

                            column.Item().Text(
                                $"Result: {result.ResultValue} {result.MeasurementUnit ?? string.Empty}");

                            column.Item().Text(
                                $"Reference range: {referenceRange}");

                            column.Item().Text(
                                $"Flag: {resultFlag}");

                            column.Item().Text(
                                $"Captured: {result.CapturedDateTime:dd MMM yyyy HH:mm} UTC");

                            if (!string.IsNullOrWhiteSpace(result.Notes))
                            {
                                column.Item().Text(
                                    $"Notes: {result.Notes}");
                            }

                            column.Item()
                                .PaddingTop(12)
                                .Text(
                                    "PRELIMINARY RESULT — NOT INDEPENDENTLY VERIFIED. " +
                                    "This result is subject to review and correction. " +
                                    "It must not be represented as a final verified report.")
                                .Bold()
                                .FontColor(Colors.Red.Darken2);

                            column.Item().Text(
                                "Confidential medical information for the requesting healthcare professional only.")
                                .Italic()
                                .FontSize(8);
                        });

                    // FOOTER
                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span(
                                "NMB-HLabSys | Preliminary | Page ");

                            text.CurrentPageNumber();

                            text.Span(" of ");

                            text.TotalPages();
                        });
                });
            });

            return document.GeneratePdf();
        }

        // ==========================================
        // 2. FINAL VERIFIED COMBINED REPORT
        // ==========================================

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

            if (results.Any(r =>
                r == null ||
                string.IsNullOrWhiteSpace(r.ResultValue) ||
                r.RequiresReview ||
                r.ResultValue.Trim().StartsWith(
                    "DEMO - NO CLINICAL RESULT",
                    StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    "The final report contains missing, demonstration or review-required results.");
            }

            // Every result must have an independent verifier.
            bool IsVerified(TestResultModel r) =>
                !string.IsNullOrWhiteSpace(
                    r.VerifiedByTechnicianId) &&
                r.VerifiedDateTime.HasValue &&
                !string.IsNullOrWhiteSpace(
                    r.TechnicianId) &&
                r.VerifiedByTechnicianId != r.TechnicianId;

            bool allVerified = results.All(IsVerified);

            // CRITICAL SAFETY CHECK
            if (!allVerified)
            {
                throw new InvalidOperationException(
                    "A final laboratory report cannot be generated " +
                    "until every result has been independently verified.");
            }

            var generatedUtc = DateTime.UtcNow;

            return Document.Create(doc =>
            {
                doc.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(35);

                    page.DefaultTextStyle(x =>
                        x.FontSize(10)
                         .FontColor(Colors.Grey.Darken3));

                    // HEADER
                    page.Header().Column(h =>
                    {
                        h.Spacing(5);

                        h.Item()
                            .Text("NMB HEALTH LABORATORY SYSTEM")
                            .Bold()
                            .FontSize(18)
                            .FontColor(Colors.Blue.Darken3);

                        h.Item()
                            .Text("VERIFIED LABORATORY RESULTS REPORT")
                            .Bold()
                            .FontSize(12)
                            .FontColor(Colors.Blue.Darken3);

                        h.Item()
                            .PaddingTop(8)
                            .LineHorizontal(1)
                            .LineColor(Colors.Grey.Lighten1);
                    });

                    // CONTENT
                    page.Content()
                        .PaddingVertical(15)
                        .Column(c =>
                        {
                            c.Spacing(12);

                            c.Item()
                                .Background(Colors.Grey.Lighten4)
                                .Padding(12)
                                .Column(info =>
                                {
                                    info.Spacing(5);

                                    info.Item().Text(
                                        $"Request ID: {testRequestId}");

                                    info.Item().Text(
                                        $"Patient: {patientName}");

                                    info.Item().Text(
                                        $"Requesting Doctor: {doctorName}");

                                    info.Item().Text(
                                        $"Generated (UTC): {generatedUtc:dd MMM yyyy HH:mm}");
                                });

                            c.Item()
                                .Text("LABORATORY TEST RESULTS")
                                .Bold()
                                .FontSize(12)
                                .FontColor(Colors.Blue.Darken3);

                            // RESULTS TABLE
                            c.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2.0f);
                                    columns.RelativeColumn(1.3f);
                                    columns.RelativeColumn(0.9f);
                                    columns.RelativeColumn(1.3f);
                                    columns.RelativeColumn(1.0f);
                                    columns.RelativeColumn(1.7f);
                                });

                                table.Header(header =>
                                {
                                    void Head(string value) =>
                                        header.Cell()
                                            .Background(
                                                Colors.Blue.Darken3)
                                            .Padding(6)
                                            .Text(value)
                                            .Bold()
                                            .FontSize(7)
                                            .FontColor(Colors.White);

                                    Head("Test / Result");
                                    Head("Value");
                                    Head("Unit");
                                    Head("Reference");
                                    Head("Flag");
                                    Head("Verification");
                                });

                                foreach (var r in results)
                                {
                                    var reference =
                                        r.ReferenceMin.HasValue &&
                                        r.ReferenceMax.HasValue
                                            ? $"{r.ReferenceMin} - {r.ReferenceMax}"
                                            : "Not available";

                                    var flag =
                                        !string.IsNullOrWhiteSpace(
                                            r.ResultFlag)
                                            ? r.ResultFlag
                                            : r.IsAbnormal
                                                ? "Abnormal"
                                                : "Not evaluated";

                                    void Cell(string value) =>
                                        table.Cell()
                                            .Background(Colors.White)
                                            .BorderBottom(0.5f)
                                            .BorderColor(
                                                Colors.Grey.Lighten2)
                                            .Padding(6)
                                            .Text(value)
                                            .FontSize(7);

                                    Cell(r.ResultName ?? "Laboratory Result");
                                    Cell(r.ResultValue ?? "N/A");
                                    Cell(r.MeasurementUnit ?? "-");
                                    Cell(reference);
                                    Cell(flag);
                                    Cell("VERIFIED");
                                }
                            });

                            // VERIFICATION DETAILS
                            c.Item()
                                .Text("RESULT AND VERIFICATION DETAILS")
                                .Bold()
                                .FontSize(11)
                                .FontColor(Colors.Blue.Darken3);

                            foreach (var r in results)
                            {
                                c.Item().Column(detail =>
                                {
                                    detail.Spacing(3);

                                    detail.Item()
                                        .Text(
                                            $"{r.ResultName} (Test ID: {r.TestRequestTestTypeId})")
                                        .Bold()
                                        .FontSize(9);

                                    var verifier =
                                        r.VerifiedByTechnician?.FullName
                                        ?? r.VerifiedByTechnicianId;

                                    detail.Item()
                                        .Text(
                                            $"Verified by: {verifier} | " +
                                            $"Verified (UTC): {r.VerifiedDateTime:dd MMM yyyy HH:mm}")
                                        .FontSize(8);

                                    if (!string.IsNullOrWhiteSpace(r.Notes))
                                    {
                                        detail.Item().Text(
                                            $"Notes: {r.Notes}")
                                            .FontSize(8);
                                    }
                                });
                            }

                            c.Item()
                                .PaddingTop(8)
                                .Text(
                                    "Confidential laboratory information intended only for the authorized requesting healthcare professional.")
                                .Italic()
                                .FontSize(8);
                        });

                    // FOOTER
                    page.Footer()
                        .AlignCenter()
                        .Text(t =>
                        {
                            t.Span("NMB-HLabSys | Page ");
                            t.CurrentPageNumber();
                            t.Span(" of ");
                            t.TotalPages();
                        });
                });
            }).GeneratePdf();
        }

        // ==========================================
        // 3. PARTIAL VERIFIED REPORT (PORTAL ONLY)
        // ==========================================

        public byte[] GeneratePartialVerifiedReport(
            int testRequestId,
            string patientName,
            string doctorName,
            List<TestResultModel> verifiedResults,
            List<int> missingTestIds)
        {
            if (verifiedResults == null ||
                verifiedResults.Count == 0)
            {
                throw new InvalidOperationException(
                    "At least one verified result is required.");
            }

            if (missingTestIds == null ||
                missingTestIds.Count == 0)
            {
                throw new InvalidOperationException(
                    "Use the final report workflow for complete requests.");
            }

            if (verifiedResults.Any(r =>
                string.IsNullOrWhiteSpace(r.ResultValue) ||
                string.IsNullOrWhiteSpace(
                    r.VerifiedByTechnicianId) ||
                !r.VerifiedDateTime.HasValue ||
                string.IsNullOrWhiteSpace(r.TechnicianId) ||
                r.VerifiedByTechnicianId == r.TechnicianId ||
                r.RequiresReview))
            {
                throw new InvalidOperationException(
                    "Unverified or review-required results cannot appear in this report.");
            }

            var generated = DateTime.UtcNow;

            return Document.Create(doc =>
            {
                doc.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(35);

                    page.DefaultTextStyle(s =>
                        s.FontSize(10));

                    // HEADER
                    page.Header().Column(h =>
                    {
                        h.Item()
                            .Text("NMB HEALTH LABORATORY SYSTEM")
                            .Bold()
                            .FontSize(17)
                            .FontColor(Colors.Blue.Darken3);

                        h.Item()
                            .Text(
                                "PARTIAL VERIFIED RESULTS — INCOMPLETE REQUEST RECORD")
                            .Bold()
                            .FontSize(12)
                            .FontColor(Colors.Red.Darken2);

                        h.Item()
                            .Text(
                                "NOT A COMPLETE FINAL LABORATORY REPORT")
                            .Bold()
                            .FontColor(Colors.Red.Darken2);

                        h.Item()
                            .PaddingTop(8)
                            .LineHorizontal(1);
                    });

                    // CONTENT
                    page.Content()
                        .PaddingVertical(16)
                        .Column(c =>
                        {
                            c.Spacing(10);

                            c.Item().Text(
                                $"Request: #{testRequestId}");

                            c.Item().Text(
                                $"Patient: {patientName}");

                            c.Item().Text(
                                $"Requesting doctor: {doctorName}");

                            c.Item().Text(
                                $"Generated (UTC): {generated:dd MMM yyyy HH:mm}");

                            c.Item()
                                .Text(
                                    $"Recorded verified results: {verifiedResults.Count}")
                                .Bold();

                            foreach (var r in verifiedResults)
                            {
                                c.Item()
                                    .Background(
                                        Colors.Grey.Lighten4)
                                    .Padding(9)
                                    .Column(x =>
                                    {
                                        x.Item()
                                            .Text(
                                                $"Test ID: {r.TestRequestTestTypeId} — {r.ResultName}")
                                            .Bold();

                                        x.Item().Text(
                                            $"Result: {r.ResultValue} {r.MeasurementUnit ?? ""}");

                                        x.Item().Text(
                                            $"Flag: {r.ResultFlag ?? (r.IsAbnormal ? "Abnormal" : "Not evaluated")}");

                                        x.Item().Text(
                                            $"Verified by: {r.VerifiedByTechnician?.FullName ?? r.VerifiedByTechnicianId}");

                                        x.Item().Text(
                                            $"Verified (UTC): {r.VerifiedDateTime:dd MMM yyyy HH:mm}");

                                        if (!string.IsNullOrWhiteSpace(r.Notes))
                                        {
                                            x.Item().Text(
                                                $"Notes: {r.Notes}");
                                        }
                                    });
                            }

                            c.Item()
                                .PaddingTop(10)
                                .Text(
                                    "TESTS WITHOUT RECORDED VERIFIED RESULTS")
                                .Bold()
                                .FontColor(Colors.Red.Darken2);

                            foreach (var id in missingTestIds.OrderBy(x => x))
                            {
                                c.Item().Text(
                                    $"Individual test ID {id}: no valid independently verified result record available");
                            }

                            c.Item()
                                .PaddingTop(8)
                                .Text(
                                    "This document reports only the recorded independently verified results shown above. " +
                                    "Other tests in the request lack valid result records. " +
                                    "This is NOT a complete final report.")
                                .Bold()
                                .FontColor(Colors.Red.Darken2);
                        });

                    // FOOTER
                    page.Footer()
                        .AlignCenter()
                        .Text(
                            "NMB-HLabSys | Confidential | Partial verified results");
                });
            }).GeneratePdf();
        }
    }
}
