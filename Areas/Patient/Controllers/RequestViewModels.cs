using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Data;

namespace NMB_HLabSys.Areas.Patient.Controllers
{
    public class RequestRow
    {
        public int Id { get; set; }
        public string RequestNumber { get; set; } = "";
        public DateTime Date { get; set; }
        public string Doctor { get; set; } = "";
        public string Urgency { get; set; } = "";
        public string Status { get; set; } = "";
        public int TestCount { get; set; }
    }

    public class RequestTestRow
    {
        public string TestName { get; set; } = "";
        public string Category { get; set; } = "";
        public decimal? Value { get; set; }
        public string Unit { get; set; } = "";
        public string Range { get; set; } = "";
        public bool Abnormal { get; set; }
    }

    public class RequestDetailsViewModel
    {
        public int Id { get; set; }
        public string RequestNumber { get; set; } = "";
        public DateTime RequestDate { get; set; }
        public string Doctor { get; set; } = "";
        public string Urgency { get; set; } = "";
        public string Status { get; set; } = "";
        public string ClinicalNotes { get; set; } = "";
        public bool Released { get; set; }
        public bool Cancelled { get; set; }
        public int Step { get; set; }
        public int AbnormalCount { get; set; }
        public List<RequestTestRow> Tests { get; set; } = new List<RequestTestRow>();
    }

    public static class RequestQueries
    {
        public static string DoctorName(string? fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "Unknown doctor";
            var n = fullName.Trim();
            var hasTitle = n.StartsWith("Dr ", StringComparison.OrdinalIgnoreCase)
                           || n.StartsWith("Dr. ", StringComparison.OrdinalIgnoreCase);
            return hasTitle ? n : "Dr. " + n;
        }

        public static async Task<List<RequestRow>> LoadRowsAsync(
            ApplicationDbContext context, string userId, string[]? statuses)
        {
            // STEP 1: Load EVERYTHING once with plain ToListAsync — no lambda inside EF
            var allRequests = await context.TestRequests.ToListAsync();
            var allItems = await context.TestRequestTestTypes.ToListAsync();
            var allUsers = await context.Users.ToListAsync();

            // STEP 2: Filter in memory
            var requests = allRequests.Where(r => r.PatientId == userId).ToList();
            if (statuses != null)
                requests = requests.Where(r => statuses.Contains(r.Status)).ToList();
            requests = requests.OrderByDescending(r => r.RequestDate).ToList();

            if (!requests.Any())
                return new List<RequestRow>();

            // STEP 3: Get item counts in memory
            var requestIds = new HashSet<int>();
            foreach (var r in requests)
                requestIds.Add(r.TestRequestId);

            var countMap = new Dictionary<int, int>();
            foreach (var item in allItems)
            {
                if (requestIds.Contains(item.TestRequestId))
                {
                    if (countMap.ContainsKey(item.TestRequestId))
                        countMap[item.TestRequestId]++;
                    else
                        countMap[item.TestRequestId] = 1;
                }
            }

            // STEP 4: Get doctor names in memory
            var doctorMap = new Dictionary<string, string>();
            foreach (var u in allUsers)
            {
                var name = (u.FirstName ?? "") + " " + (u.LastName ?? "");
                doctorMap[u.Id] = DoctorName(name);
            }

            // STEP 5: Build result
            var result = new List<RequestRow>();
            foreach (var r in requests)
            {
                result.Add(new RequestRow
                {
                    Id = r.TestRequestId,
                    RequestNumber = r.RequestNumber ?? "",
                    Date = r.RequestDate,
                    Doctor = doctorMap.ContainsKey(r.DoctorId) ? doctorMap[r.DoctorId] : "Unknown doctor",
                    Urgency = r.Urgency.ToString(),
                    Status = r.Status ?? "",
                    TestCount = countMap.ContainsKey(r.TestRequestId) ? countMap[r.TestRequestId] : 0
                });
            }

            return result;
        }
    }
}