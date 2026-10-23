using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Patient.Models;
using NMB_HLabSys.Data;
using System.Security.Claims;

namespace NMB_HLabSys.Areas.Patient.Controllers
{
    [Area("Patient")]
    [Authorize(Roles = "Patient")]
    public class ConsentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConsentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // INDEX — Doctor Access page
        // ============================================================
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return RedirectToAction("Index", "Home");

            // Load ALL consents in memory (avoids Contains() JIT bug)
            var allConsents = await _context.PatientDoctorConsents.ToListAsync();

            // Only ACTIVE consents appear on the page
            var consents = allConsents
                .Where(c => c.PatientId == patient.PatientId && c.IsActive)
                .ToList();

            // Load ALL users in memory
            var allUsers = await _context.Users.ToListAsync();
            var userMap = new Dictionary<string, string>();
            foreach (var u in allUsers)
            {
                var name = ((u.FirstName ?? "") + " " + (u.LastName ?? "")).Trim();
                if (string.IsNullOrEmpty(name)) name = u.UserName ?? u.Email ?? "Unknown";
                userMap[u.Id] = name;
            }

            // Load ALL roles in memory
            var allRoles = await _context.Roles.ToListAsync();
            var doctorRole = allRoles.FirstOrDefault(r => r.Name == "Doctor");

            var doctorUserIds = new List<string>();
            if (doctorRole != null)
            {
                var allUserRoles = await _context.UserRoles.ToListAsync();
                doctorUserIds = allUserRoles
                    .Where(ur => ur.RoleId == doctorRole.Id)
                    .Select(ur => ur.UserId)
                    .ToList();
            }

            // Doctors already granted active access cannot be granted again
            var alreadyGranted = consents
                .Select(c => c.DoctorUserId)
                .Where(id => id != null)
                .ToHashSet();

            // Available doctors = doctor-role users not already granted
            var availableDoctors = new List<(string Id, string Name)>();
            foreach (var docId in doctorUserIds)
            {
                if (!alreadyGranted.Contains(docId) && userMap.ContainsKey(docId))
                {
                    availableDoctors.Add((docId, userMap[docId]));
                }
            }

            // Test requests for this patient (in memory)
            var allRequests = await _context.TestRequests.ToListAsync();
            var testRequests = allRequests
                .Where(r => r.PatientId == patient.UserId)
                .OrderByDescending(r => r.RequestDate)
                .ToList();

            // Build consent rows (only active ones, since we filtered above)
            var consentRows = new List<ConsentRow>();
            foreach (var c in consents)
            {
                consentRows.Add(new ConsentRow
                {
                    ConsentId = c.PatientDoctorConsentId,
                    DoctorName = c.DoctorUserId != null && userMap.ContainsKey(c.DoctorUserId)
                        ? userMap[c.DoctorUserId] : "Unknown Doctor",
                    GrantedDate = c.GrantedDate,
                    RevokedDate = c.RevokedDate,
                    IsActive = c.IsActive,
                    Notes = c.Notes ?? ""
                });
            }

            // Build consent items (only for active consents)
            var allItems = await _context.ConsentRequestItems.ToListAsync();
            var activeConsentIds = consentRows.Select(r => r.ConsentId).ToHashSet();
            var itemsByConsent = new Dictionary<int, List<ConsentItemRow>>();

            foreach (var item in allItems)
            {
                if (!activeConsentIds.Contains(item.PatientConsentId)) continue;

                if (!itemsByConsent.ContainsKey(item.PatientConsentId))
                    itemsByConsent[item.PatientConsentId] = new List<ConsentItemRow>();

                var req = allRequests.FirstOrDefault(r => r.TestRequestId == item.TestRequestId);
                itemsByConsent[item.PatientConsentId].Add(new ConsentItemRow
                {
                    ItemId = item.ConsentRequestItemId,
                    RequestNumber = req?.RequestNumber ?? "Unknown Request",
                    RequestDate = req?.RequestDate ?? DateTime.MinValue,
                    ClinicalNotes = req?.ClinicalNotes ?? ""
                });
            }

            ViewBag.PatientName = patient.Name + " " + patient.Surname;
            ViewBag.Consents = consentRows;
            ViewBag.ConsentItems = itemsByConsent;
            ViewBag.Doctors = availableDoctors;
            ViewBag.TestRequests = testRequests;

            return View();
        }

        // ============================================================
        // GRANT CONSENT — creates a new active consent + optional items
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GrantConsent(string doctorUserId, List<int>? selectedRequestIds)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return RedirectToAction("Index", "Home");

            if (string.IsNullOrWhiteSpace(doctorUserId))
            {
                TempData["Error"] = "Please choose a doctor first.";
                return RedirectToAction("Index");
            }

            try
            {
                // Only users with role "Doctor" can be granted access
                var isDoctor = await (from ur in _context.UserRoles
                                      join ro in _context.Roles on ur.RoleId equals ro.Id
                                      where ur.UserId == doctorUserId && ro.Name == "Doctor"
                                      select ur).AnyAsync();
                if (!isDoctor)
                {
                    TempData["Error"] = "That user is not a doctor.";
                    return RedirectToAction("Index");
                }

                // Load existing consents in memory
                var mine = await _context.PatientDoctorConsents
                    .Where(c => c.PatientId == patient.PatientId)
                    .ToListAsync();

                if (mine.Any(c => c.IsActive && c.DoctorUserId == doctorUserId))
                {
                    TempData["Error"] = "This doctor already has access.";
                    return RedirectToAction("Index");
                }

                var consent = new PatientDoctorConsentModel
                {
                    PatientId = patient.PatientId,
                    DoctorUserId = doctorUserId,
                    GrantedDate = DateTime.Now,
                    IsActive = true,
                    Notes = "Access granted by patient"
                };
                _context.PatientDoctorConsents.Add(consent);
                await _context.SaveChangesAsync();

                // Patient can only share their own requests
                var myRequestIds = await _context.TestRequests
                    .Where(r => r.PatientId == patient.UserId)
                    .Select(r => r.TestRequestId)
                    .ToListAsync();

                var chosen = (selectedRequestIds ?? new List<int>())
                    .Distinct()
                    .Where(id => myRequestIds.Contains(id))
                    .ToList();

                foreach (var requestId in chosen)
                {
                    _context.ConsentRequestItems.Add(new ConsentRequestItemModel
                    {
                        PatientConsentId = consent.PatientDoctorConsentId,
                        TestRequestId = requestId
                    });
                }
                if (chosen.Count > 0) await _context.SaveChangesAsync();

                TempData["Success"] = chosen.Count > 0
                    ? "Access granted for " + chosen.Count + " test request(s)."
                    : "Access granted. You can choose which records this doctor sees at any time.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Access could not be granted: " + (ex.InnerException?.Message ?? ex.Message);
            }

            return RedirectToAction("Index");
        }

        // ============================================================
        // REVOKE ALL — sets IsActive = false + RevokedDate
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RevokeConsent(int consentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return RedirectToAction("Index", "Home");

            var consent = await _context.PatientDoctorConsents
                .FirstOrDefaultAsync(c => c.PatientDoctorConsentId == consentId
                                       && c.PatientId == patient.PatientId);

            if (consent != null)
            {
                consent.IsActive = false;
                consent.RevokedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Access revoked successfully.";
            }

            return RedirectToAction("Index");
        }

        // ============================================================
        // REVOKE SINGLE RECORD
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RevokeConsentItem(int itemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return RedirectToAction("Index", "Home");

            var allConsents = await _context.PatientDoctorConsents.ToListAsync();
            var myConsentIds = allConsents
                .Where(c => c.PatientId == patient.PatientId)
                .Select(c => c.PatientDoctorConsentId)
                .ToHashSet();

            var item = await _context.ConsentRequestItems.FindAsync(itemId);
            if (item != null && myConsentIds.Contains(item.PatientConsentId))
            {
                _context.ConsentRequestItems.Remove(item);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Record access revoked.";
            }

            return RedirectToAction("Index");
        }
    }

    // ============================================================
    // VIEW MODELS
    // ============================================================
    public class ConsentRow
    {
        public int ConsentId { get; set; }
        public string DoctorName { get; set; } = "";
        public DateTime GrantedDate { get; set; }
        public DateTime? RevokedDate { get; set; }
        public bool IsActive { get; set; }
        public string Notes { get; set; } = "";
    }

    public class ConsentItemRow
    {
        public int ItemId { get; set; }
        public string RequestNumber { get; set; } = "";
        public DateTime RequestDate { get; set; }
        public string ClinicalNotes { get; set; } = "";
    }
}