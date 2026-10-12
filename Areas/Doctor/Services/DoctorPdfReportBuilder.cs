using System.Text;

namespace NMB_HLabSys.Areas.Doctor.Services;

/// <summary>Small dependency-free PDF generator for the Doctor request reports.</summary>
public static class DoctorPdfReportBuilder
{
    public static byte[] Build(string title, string generatedAt, string criteria, IReadOnlyList<string[]> rows, string emptyMessage)
    {
        const int rowsPerPage = 32;
        var pages = Math.Max(1, (int)Math.Ceiling(Math.Max(rows.Count, 1) / (double)rowsPerPage));
        var objects = new List<string>();
        objects.Add("<< /Type /Catalog /Pages 2 0 R >>"); // 1
        objects.Add(""); // pages object 2 assigned later
        objects.Add("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>"); // 3
        objects.Add("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >>"); // 4
        var pageIds = new List<int>();
        for (var p = 0; p < pages; p++)
        {
            var content = new StringBuilder();
            Text(content, 42, 800, title, true, 16);
            Text(content, 42, 782, "NMB-HLabSys | Doctor subsystem", false, 9);
            Text(content, 42, 767, "Generated: " + generatedAt, false, 9);
            Text(content, 42, 752, "Criteria: " + criteria, false, 9);
            content.Append("0.65 w 42 740 m 553 740 l S\n");
            Text(content, 42, 722, "Request number", true, 9);
            Text(content, 165, 722, "Patient", true, 9);
            Text(content, 300, 722, "Request date", true, 9);
            Text(content, 390, 722, "Urgency", true, 9);
            Text(content, 455, 722, "Status", true, 9);
            content.Append("0.35 w 42 715 m 553 715 l S\n");
            var start = p * rowsPerPage;
            var count = Math.Min(rowsPerPage, rows.Count - start);
            if (rows.Count == 0) Text(content, 42, 690, emptyMessage, false, 10);
            else
            {
                for (var i = 0; i < count; i++)
                {
                    var y = 697 - i * 19;
                    var r = rows[start + i];
                    Text(content, 42, y, Cell(r, 0, 20), false, 8);
                    Text(content, 165, y, Cell(r, 1, 22), false, 8);
                    Text(content, 300, y, Cell(r, 2, 15), false, 8);
                    Text(content, 390, y, Cell(r, 3, 10), false, 8);
                    Text(content, 455, y, Cell(r, 4, 15), false, 8);
                    content.Append($"0.15 w 42 {y - 5} m 553 {y - 5} l S\n");
                }
            }
            content.Append("0.5 w 42 45 m 553 45 l S\n");
            Text(content, 42, 29, "Confidential patient information - handle according to organisational policy.", false, 8);
            Text(content, 475, 29, $"Page {p + 1} of {pages}", false, 8);
            var contentId = objects.Count + 1; objects.Add($"<< /Length {content.Length} >>\nstream\n{content}endstream");
            var pageId = objects.Count + 1; pageIds.Add(pageId);
            objects.Add($"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources << /Font << /F1 3 0 R /F2 4 0 R >> >> /Contents {contentId} 0 R >>");
        }
        objects[1] = $"<< /Type /Pages /Kids [{string.Join(" ", pageIds.Select(id => id + " 0 R"))}] /Count {pages} >>";
        var pdf = new StringBuilder("%PDF-1.4\n%\xE2\xE3\xCF\xD3\n");
        var offsets = new List<int> { 0 };
        for (var i = 0; i < objects.Count; i++)
        {
            offsets.Add(pdf.Length);
            pdf.Append($"{i + 1} 0 obj\n{objects[i]}\nendobj\n");
        }
        var xref = pdf.Length;
        pdf.Append($"xref\n0 {objects.Count + 1}\n0000000000 65535 f \n");
        for (var i = 1; i < offsets.Count; i++) pdf.Append(offsets[i].ToString("D10") + " 00000 n \n");
        pdf.Append($"trailer\n<< /Size {objects.Count + 1} /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF");
        return Encoding.ASCII.GetBytes(pdf.ToString());
    }

    private static string Cell(string[] row, int index, int max) => Fit(index < row.Length ? row[index] : "", max);
    private static string Fit(string value, int max)
    {
        var s = new string((value ?? "").Select(c => c switch { '\r' or '\n' or '\t' => ' ', >= ' ' and <= '~' => c, '–' or '—' => '-', _ => '?' }).ToArray());
        return s.Length > max ? s[..(max - 1)] + "." : s;
    }
    private static void Text(StringBuilder b, int x, int y, string value, bool bold, int size)
    {
        var safe = Fit(value, 110).Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
        b.Append($"BT /{(bold ? "F2" : "F1")} {size} Tf {x} {y} Td ({safe}) Tj ET\n");
    }
}
