
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace NMB_HLabSys.Areas.Doctor.Services;

public static class DoctorPdfReportBuilder
{
    private const double PageWidth = 842;
    private const double PageHeight = 595;
    private const int RowsPerPage = 20;
    private const double Left = 38;
    private const double Right = 804;

    public static byte[] Build(string title, string generatedAt, string criteria,
        IReadOnlyList<string[]> rows, string emptyMessage)
        => Build(title, "Not specified", null, generatedAt, criteria, rows, emptyMessage);

    public static byte[] Build(string title, string doctorName, string? hpcsaNumber,
        string generatedAt, string criteria, IReadOnlyList<string[]> rows, string emptyMessage)
    {
        rows ??= Array.Empty<string[]>();

        var safeRows = rows.Select(row => new[]
        {
            Value(row, 0),
            Value(row, 1),
            Value(row, 2),
            Value(row, 3),
            Value(row, 4)
        }).ToList();

        var pageCount = Math.Max(
            1,
            (int)Math.Ceiling(Math.Max(safeRows.Count, 1) / (double)RowsPerPage));

        var logoBytes = LoadLogoBytes();

        var objects = new List<byte[]>
        {
            Ascii("<< /Type /Catalog /Pages 2 0 R >>"),
            Array.Empty<byte>(),
            Ascii("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>"),
            Ascii("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >>")
        };

        if (logoBytes != null)
        {
            var imageHeader = Ascii(
                $"<< /Type /XObject /Subtype /Image /Width 360 /Height 240 " +
                $"/ColorSpace /DeviceRGB /BitsPerComponent 8 /Filter /DCTDecode " +
                $"/Length {logoBytes.Length} >>\nstream\n");

            objects.Add(JoinBytes(imageHeader, logoBytes, Ascii("\nendstream")));
        }

        var pageObjectNumbers = new List<int>();

        for (var pageIndex = 0; pageIndex < pageCount; pageIndex++)
        {
            var pageNumber = pageIndex + 1;
            var pageRows = safeRows
                .Skip(pageIndex * RowsPerPage)
                .Take(RowsPerPage)
                .ToList();

            var content = BuildPage(
                title,
                doctorName,
                hpcsaNumber,
                generatedAt,
                criteria,
                pageRows,
                safeRows.Count,
                emptyMessage,
                pageNumber,
                pageCount,
                logoBytes != null);

            var contentBytes = Ascii(content);
            var pageObjectNumber = objects.Count + 1;
            var contentObjectNumber = pageObjectNumber + 1;

            pageObjectNumbers.Add(pageObjectNumber);

            objects.Add(Ascii(
                $"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {PageWidth} {PageHeight}] " +
                $"/Resources << /Font << /F1 3 0 R /F2 4 0 R >> " +
                (logoBytes != null ? "/XObject << /Logo 5 0 R >> " : "") +
                ">> " +
                $"/Contents {contentObjectNumber} 0 R >>"));

            objects.Add(JoinBytes(
                Ascii($"<< /Length {contentBytes.Length} >>\nstream\n"),
                contentBytes,
                Ascii("\nendstream")));
        }

        var kids = string.Join(" ", pageObjectNumbers.Select(n => $"{n} 0 R"));

        objects[1] = Ascii(
            $"<< /Type /Pages /Kids [{kids}] /Count {pageCount} >>");

        return BuildPdf(objects);
    }

    private static string BuildPage(
        string title,
        string doctorName,
        string? hpcsaNumber,
        string generatedAt,
        string criteria,
        IReadOnlyList<string[]> rows,
        int totalRecords,
        string emptyMessage,
        int pageNumber,
        int pageCount,
        bool hasLogo)
    {
        var sb = new StringBuilder();

        Rect(sb, 0, 0, PageWidth, PageHeight, 1, 1, 1);
        Rect(sb, 0, 515, PageWidth, 80, 0.04, 0.25, 0.40);

        // Logo: raised and vertically aligned with the heading text.
        // Original image proportions are preserved.
        if (hasLogo)
            sb.AppendLine("q 48 0 0 32 38 539 cm /Logo Do Q");

        Text(sb, hasLogo ? 92 : 38, 563, "NMB-HLabSys",
            19, true, 1, 1, 1);

        Text(sb, hasLogo ? 92 : 38, 540,
            "NELSON MANDELA BAY HAEMATOLOGY LABORATORY SYSTEM",
            8, false, 0.88, 0.94, 0.97);

        Text(sb, 804, 562, Clean(title),
            15, true, 1, 1, 1, rightAligned: true);

        Text(sb, 804, 540, "CONFIDENTIAL SYSTEM REPORT",
            8, true, 0.88, 0.94, 0.97, rightAligned: true);

        Text(sb, Left, 493, "REPORTING DOCTOR",
            8, true, 0.35, 0.42, 0.48);

        Text(sb, Left, 475, Clean(doctorName),
            12, true, 0.08, 0.15, 0.22);

        if (!string.IsNullOrWhiteSpace(hpcsaNumber))
        {
            Text(sb, Left, 459, $"HPCSA number: {Clean(hpcsaNumber)}",
                8, false, 0.30, 0.36, 0.42);
        }

        Text(sb, 480, 493, "GENERATED",
            8, true, 0.35, 0.42, 0.48);

        Text(sb, 480, 475, Clean(generatedAt),
            9, false, 0.08, 0.15, 0.22);

        Text(sb, 650, 493, "TOTAL REQUESTS",
            8, true, 0.35, 0.42, 0.48);

        Text(sb, 650, 473, totalRecords.ToString(CultureInfo.InvariantCulture),
            17, true, 0.02, 0.40, 0.60);

        Line(sb, Left, 446, Right, 446, 0.82, 0.87, 0.90, 0.8);

        Text(sb, Left, 428, "REPORT CRITERIA",
            8, true, 0.04, 0.35, 0.52);

        var criteriaLines = Wrap(Clean(criteria), 115).Take(2).ToList();

        if (criteriaLines.Count == 0)
            criteriaLines.Add("No filters applied.");

        var criteriaY = 412.0;

        foreach (var line in criteriaLines)
        {
            Text(sb, Left, criteriaY, line,
                8, false, 0.22, 0.28, 0.34);

            criteriaY -= 12;
        }

        var statusCounts = rows
            .GroupBy(r => NormalizeStatus(r[4]))
            .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var summaryY = 375.0;

        Text(sb, Left, summaryY, "STATUS SUMMARY",
            8, true, 0.04, 0.35, 0.52);

        var summary = statusCounts.Count == 0
            ? "No request statuses to summarise."
            : string.Join("    |    ",
                statusCounts.Select(g => $"{g.Key}: {g.Count()}"));

        foreach (var line in Wrap(summary, 130).Take(2))
        {
            summaryY -= 13;

            Text(sb, Left, summaryY, line,
                8, false, 0.22, 0.28, 0.34);
        }

        var tableTop = summaryY - 18;
        var headerBottom = tableTop - 27;

        Rect(sb, Left, headerBottom, Right - Left, 27,
            0.04, 0.25, 0.40);

        Text(sb, 46, tableTop - 17, "REQUEST NUMBER",
            8, true, 1, 1, 1);

        Text(sb, 222, tableTop - 17, "PATIENT",
            8, true, 1, 1, 1);

        Text(sb, 405, tableTop - 17, "REQUEST DATE",
            8, true, 1, 1, 1);

        Text(sb, 520, tableTop - 17, "URGENCY",
            8, true, 1, 1, 1);

        Text(sb, 625, tableTop - 17, "STATUS",
            8, true, 1, 1, 1);

        var y = headerBottom - 22;

        if (totalRecords == 0)
        {
            Rect(sb, Left, y - 24, Right - Left, 44,
                0.94, 0.97, 0.98);

            Text(sb, Left + 14, y, Clean(emptyMessage),
                10, false, 0.20, 0.29, 0.36);
        }
        else
        {
            foreach (var row in rows)
            {
                if ((int)Math.Round(y) < 68)
                    break;

                var rowBottom = y - 8;

                if (((int)Math.Round(y)) % 2 == 0)
                {
                    Rect(sb, Left, rowBottom, Right - Left, 25,
                        0.97, 0.98, 0.99);
                }

                Text(sb, 46, y, Fit(row[0], 28),
                    8, false, 0.10, 0.16, 0.22);

                Text(sb, 222, y, Fit(row[1], 27),
                    8, false, 0.10, 0.16, 0.22);

                Text(sb, 405, y, Fit(row[2], 17),
                    8, false, 0.10, 0.16, 0.22);

                Text(sb, 520, y, Fit(row[3], 14),
                    8, false, 0.10, 0.16, 0.22);

                Text(sb, 625, y, Fit(NormalizeStatus(row[4]), 24),
                    8, true, 0.04, 0.35, 0.52);

                Line(sb, Left, rowBottom, Right, rowBottom,
                    0.88, 0.91, 0.93, 0.45);

                y -= 26;
            }
        }

        Line(sb, Left, 48, Right, 48, 0.82, 0.87, 0.90, 0.7);

        Text(sb, Left, 31,
            "CONFIDENTIAL - Patient information must be handled according to organisational policy.",
            7, false, 0.35, 0.40, 0.45);

        Text(sb, Right, 31, $"Page {pageNumber} of {pageCount}",
            8, true, 0.20, 0.28, 0.34, rightAligned: true);

        return sb.ToString();
    }

    private static string NormalizeStatus(string value)
    {
        var status = Clean(value).Trim();

        return status.Replace("_", " ") switch
        {
            "InProgress" => "In Progress",
            "SamplesReceived" => "Samples Received",
            "ReleasedByDoctor" => "Released by Doctor",
            "Submitted" => "Submitted",
            "Completed" => "Completed",
            "Released" => "Released",
            "Cancelled" => "Cancelled",
            _ => status
        };
    }

    private static string Value(string[]? row, int index)
        => row != null && index < row.Length
            ? row[index] ?? string.Empty
            : string.Empty;

    private static string Clean(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var normalized = value.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) ==
                UnicodeCategory.NonSpacingMark)
                continue;

            if (c == '\r' || c == '\n' || c == '\t')
                sb.Append(' ');
            else if (c >= 32 && c <= 126)
                sb.Append(c);
            else if (c == '\u2013' || c == '\u2014')
                sb.Append('-');
            else if (c == '\u2019' || c == '\u2018')
                sb.Append('\'');
            else if (c == '\u201c' || c == '\u201d')
                sb.Append('"');
            else
                sb.Append('?');
        }

        return sb.ToString();
    }

    private static string Fit(string value, int maxLength)
    {
        value = Clean(value);

        if (value.Length <= maxLength)
            return value;

        if (maxLength <= 3)
            return value[..maxLength];

        return value[..(maxLength - 3)] + "...";
    }

    private static IEnumerable<string> Wrap(string value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            yield return string.Empty;
            yield break;
        }

        var words = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var line = new StringBuilder();

        foreach (var originalWord in words)
        {
            var word = originalWord;

            while (word.Length > maxLength)
            {
                if (line.Length > 0)
                {
                    yield return line.ToString();
                    line.Clear();
                }

                yield return word[..maxLength];
                word = word[maxLength..];
            }

            if (line.Length > 0 && line.Length + word.Length + 1 > maxLength)
            {
                yield return line.ToString();
                line.Clear();
            }

            if (line.Length > 0)
                line.Append(' ');

            line.Append(word);
        }

        if (line.Length > 0)
            yield return line.ToString();
    }

    private static void Text(
        StringBuilder sb,
        double x,
        double y,
        string text,
        int size,
        bool bold,
        double r,
        double g,
        double b,
        bool rightAligned = false)
    {
        text = Clean(text);

        if (rightAligned)
            x -= text.Length * size * 0.48;

        sb.AppendFormat(
            CultureInfo.InvariantCulture,
            "BT /{0} {1} Tf {2:0.##} {3:0.##} {4:0.##} rg 1 0 0 1 {5:0.##} {6:0.##} Tm ({7}) Tj ET\n",
            bold ? "F2" : "F1",
            size,
            r, g, b,
            x, y,
            EscapePdf(text));
    }

    private static void Line(
        StringBuilder sb,
        double x1,
        double y1,
        double x2,
        double y2,
        double r,
        double g,
        double b,
        double width)
    {
        sb.AppendFormat(
            CultureInfo.InvariantCulture,
            "{0:0.##} {1:0.##} {2:0.##} RG {3:0.##} w {4:0.##} {5:0.##} m {6:0.##} {7:0.##} l S\n",
            r, g, b, width,
            x1, y1, x2, y2);
    }

    private static void Rect(
        StringBuilder sb,
        double x,
        double y,
        double width,
        double height,
        double r,
        double g,
        double b)
    {
        sb.AppendFormat(
            CultureInfo.InvariantCulture,
            "{0:0.##} {1:0.##} {2:0.##} rg {3:0.##} {4:0.##} {5:0.##} {6:0.##} re f\n",
            r, g, b,
            x, y, width, height);
    }

    private static string EscapePdf(string value)
        => value.Replace("\\", "\\\\")
                .Replace("(", "\\(")
                .Replace(")", "\\)");

    private static byte[] Ascii(string value)
        => Encoding.ASCII.GetBytes(value);

    private static byte[] JoinBytes(params byte[][] parts)
    {
        using var stream = new MemoryStream();

        foreach (var part in parts)
            stream.Write(part, 0, part.Length);

        return stream.ToArray();
    }

    private static byte[]? LoadLogoBytes()
    {
        var candidates = new[]
        {
            Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot", "assets", "img", "nmb-report-logo.png"),

            Path.Combine(
                AppContext.BaseDirectory,
                "wwwroot", "assets", "img", "nmb-report-logo.png"),

            Path.GetFullPath(Path.Combine(
                AppContext.BaseDirectory,
                "..", "..", "..", "..",
                "wwwroot", "assets", "img", "nmb-report-logo.png"))
        };

        var path = candidates.FirstOrDefault(File.Exists);

        if (path == null)
            return null;

        using var image = Image.Load<Rgba32>(path);

        // Flatten transparency onto the same blue used in the PDF header.
        image.Mutate(x =>
            x.BackgroundColor(SixLabors.ImageSharp.Color.Parse("#0A4066")));

        image.Mutate(x => x.Resize(360, 240));

        using var output = new MemoryStream();
        image.SaveAsJpeg(output);

        return output.ToArray();
    }

    private static byte[] BuildPdf(IReadOnlyList<byte[]> objects)
    {
        using var output = new MemoryStream();

        void Write(string value)
        {
            var bytes = Ascii(value);
            output.Write(bytes, 0, bytes.Length);
        }

        Write("%PDF-1.4\n%NMB-HLabSys\n");

        var offsets = new List<long> { 0 };

        for (var i = 0; i < objects.Count; i++)
        {
            offsets.Add(output.Position);

            Write($"{i + 1} 0 obj\n");

            var obj = objects[i];
            output.Write(obj, 0, obj.Length);

            Write("\nendobj\n");
        }

        var xrefPosition = output.Position;

        Write($"xref\n0 {objects.Count + 1}\n");
        Write("0000000000 65535 f \n");

        for (var i = 1; i < offsets.Count; i++)
            Write($"{offsets[i]:D10} 00000 n \n");

        Write(
            $"trailer\n<< /Size {objects.Count + 1} /Root 1 0 R >>\n" +
            $"startxref\n{xrefPosition}\n%%EOF");

        return output.ToArray();
    }
}
