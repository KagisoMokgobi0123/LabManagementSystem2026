
using System;
using System.Collections.Generic;
using System.Globalization;
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
                    "Cannot generate a report without results.");
            }

            if (results.Any(r =>
                !r.VerifiedDateTime.HasValue ||
                string.IsNullOrWhiteSpace(
                    r.VerifiedByTechnicianId) ||
                r.RequiresReview))
            {
                throw new InvalidOperationException(
                    "Cannot generate a report containing unverified results.");
            }

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);

                    page.DefaultTextStyle(x => x.FontSize(9));

                    // HEADER
                    page.Header().Column(column =>
                    {
                        column.Item()
                            .Text("NMB-HLabSys")
                            .FontSize(20)
                            .Bold()
                            .FontColor(Colors.Blue.Darken3);

                        column.Item()
                            .Text("VERIFIED LABORATORY RESULTS")
                            .FontSize(12)
                            .Bold();

                        column.Item()
                            .PaddingTop(8)
                            .LineHorizontal(1);
                    });

                    // REPORT CONTENT
                    page.Content()
                        .PaddingVertical(15)
                        .Column(column =>
                        {
                            column.Spacing(12);

                            column.Item().Text(
                                $"Test Request: #{testRequestId}");

                            column.Item().Text(
                                $"Patient: {patientName}");

                            column.Item().Text(
                                $"Requesting Doctor: {doctorName}");

                            column.Item().Text(
                                $"Generated: {DateTime.UtcNow:dd MMM yyyy HH:mm} UTC");

                            column.Item()
                                .PaddingTop(10)
                                .Text("LABORATORY RESULTS")
                                .Bold()
                                .FontSize(11);

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Text("Test").Bold();
                                    header.Cell().Text("Result").Bold();
                                    header.Cell().Text("Unit").Bold();
                                    header.Cell().Text("Reference").Bold();
                                    header.Cell().Text("Flag").Bold();
                                });

                                foreach (var result in results)
                                {
                                    table.Cell().Text(
                                        result.ResultName);

                                    table.Cell().Text(
                                        result.ResultValue);

                                    table.Cell().Text(
                                        result.MeasurementUnit ?? "-");

                                    table.Cell().Text(
                                        FormatReferenceRange(result));

                                    table.Cell().Text(
                                        result.ResultFlag);
                                }
                            });

                            column.Item()
                                .PaddingTop(12)
                                .Text("VERIFICATION DETAILS")
                                .Bold()
                                .FontSize(11);

                            foreach (var result in results)
                            {
                                column.Item().Column(detail =>
                                {
                                    detail.Spacing(3);

                                    detail.Item()
                                        .Text(result.ResultName)
                                        .Bold();

                                    detail.Item().Text(
                                        "Captured by: " +
                                        FormatTechnicianName(
                                            result.Technician));

                                    detail.Item().Text(
                                        "Verified by: " +
                                        FormatTechnicianName(
                                            result.VerifiedByTechnician));

                                    detail.Item().Text(
                                        "Verified on: " +
                                        result.VerifiedDateTime!
                                            .Value.ToString(
                                                "dd MMM yyyy HH:mm"));

                                    if (!string.IsNullOrWhiteSpace(
                                        result.Notes))
                                    {
                                        detail.Item().Text(
                                            "Technician Notes: " +
                                            result.Notes);
                                    }

                                    if (!string.IsNullOrWhiteSpace(
                                        result.VerificationNotes))
                                    {
                                        detail.Item().Text(
                                            "Verification Notes: " +
                                            result.VerificationNotes);
                                    }
                                });
                            }
                        });

                    // FOOTER
                    page.Footer()
                        .AlignCenter()
                        .Text("CONFIDENTIAL LABORATORY REPORT")
                        .FontSize(8);
                });
            }).GeneratePdf();
        }

        private static string FormatReferenceRange(
            TestResultModel result)
        {
            var min = result.ReferenceMin;
            var max = result.ReferenceMax;

            if (min.HasValue && max.HasValue)
            {
                return
                    $"{min.Value.ToString(CultureInfo.InvariantCulture)}" +
                    $" - {max.Value.ToString(CultureInfo.InvariantCulture)}";
            }

            if (min.HasValue)
            {
                return
                    $">= {min.Value.ToString(CultureInfo.InvariantCulture)}";
            }

            if (max.HasValue)
            {
                return
                    $"<= {max.Value.ToString(CultureInfo.InvariantCulture)}";
            }

            return "Not provided";
        }

        private static string FormatTechnicianName(
            NMB_HLabSys.Models.Identity.ApplicationUser? technician)
        {
            if (technician == null)
                return "Not available";

            var name =
                $"{technician.FirstName} {technician.LastName}".Trim();

            return string.IsNullOrWhiteSpace(name)
                ? technician.UserName ?? "Not available"
                : name;
        }
    }
}
