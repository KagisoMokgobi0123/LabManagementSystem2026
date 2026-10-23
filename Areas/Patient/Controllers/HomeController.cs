using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Patient.Models;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NMB_HLabSys.Areas.Patient.Controllers
{
    [Area("Patient")]
    [Authorize(Roles = "Patient")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<PatientModel?> GetCurrentPatient()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        // SHA256 hash — matches AccountController.HashPassword pattern
        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        // ============================================================
        // DASHBOARD
        // ============================================================
        public async Task<IActionResult> Index()
        {
            var patient = await GetCurrentPatient();
            if (patient == null) return RedirectToAction("Login", "Account", new { area = "" });

            var allRequests = await _context.TestRequests
                .Where(r => r.PatientId == patient.UserId)
                .ToListAsync();

            var conditions = await _context.PatientConditions
                .Where(pc => pc.PatientId == patient.PatientId)
                .Include(pc => pc.MedicalCondition)
                .ToListAsync();

            var allergies = await _context.PatientAllergies
                .Where(pa => pa.PatientId == patient.PatientId)
                .Include(pa => pa.Allergy)
                .ToListAsync();

            var medications = await _context.PatientMedications
                .Where(pm => pm.PatientId == patient.PatientId)
                .Include(pm => pm.Medication)
                .ToListAsync();

            var consents = await _context.PatientDoctorConsents
                .Where(c => c.PatientId == patient.PatientId && c.IsActive)
                .ToListAsync();

            var model = new DashboardViewModel
            {
                PatientId = patient.PatientId,
                PatientName = patient.Name,
                PatientSurname = patient.Surname,
                Email = patient.Email,
                ContactNumber = patient.ContactNumber,
                Address = patient.Address,
                TotalRequests = allRequests.Count,
                PendingRequests = allRequests.Count(r =>
                    r.Status == "Submitted" ||
                    r.Status == "Samples Received" ||
                    r.Status == "InProgress"),
                CompletedRequests = allRequests.Count(r => r.Status == "Completed"),
                ReleasedRequests = allRequests.Count(r => r.Status == "ReleasedByDoctor"),
                RecentRequests = allRequests.OrderByDescending(r => r.RequestDate).Take(5).ToList()
            };

            ViewBag.ConditionsCount = conditions.Count;
            ViewBag.AllergiesCount = allergies.Count;
            ViewBag.MedicationsCount = medications.Count;
            ViewBag.DoctorAccessCount = consents.Count;

            return View(model);
        }

        // ============================================================
        // MY TEST REQUESTS
        // ============================================================
        public async Task<IActionResult> Requests()
        {
            var patient = await GetCurrentPatient();
            if (patient == null) return RedirectToAction("Login", "Account", new { area = "" });

            var requests = await _context.TestRequests
                .Where(r => r.PatientId == patient.UserId)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            ViewBag.PatientName = patient.Name + " " + patient.Surname;
            return View(requests);
        }

        // ============================================================
        // MY RESULTS
        // ============================================================
        public async Task<IActionResult> Results()
        {
            var patient = await GetCurrentPatient();
            if (patient == null) return RedirectToAction("Login", "Account", new { area = "" });

            var results = await _context.TestRequests
                .Where(r => r.PatientId == patient.UserId &&
                           (r.Status == "Completed" || r.Status == "ReleasedByDoctor"))
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            ViewBag.PatientName = patient.Name + " " + patient.Surname;
            return View(results);
        }

        // ============================================================
        // RESULT HISTORY
        // ============================================================
        public async Task<IActionResult> History()
        {
            var patient = await GetCurrentPatient();
            if (patient == null) return RedirectToAction("Login", "Account", new { area = "" });

            var results = await _context.TestRequests
                .Where(r => r.PatientId == patient.UserId)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            ViewBag.PatientName = patient.Name + " " + patient.Surname;
            return View(results);
        }

        // ============================================================
        // MEDICAL HISTORY
        // ============================================================
        public async Task<IActionResult> MedicalHistory()
        {
            var patient = await GetCurrentPatient();
            if (patient == null) return RedirectToAction("Login", "Account", new { area = "" });

            var conditions = await _context.PatientConditions
                .Where(pc => pc.PatientId == patient.PatientId)
                .Include(pc => pc.MedicalCondition)
                .ToListAsync();

            var allergies = await _context.PatientAllergies
                .Where(pa => pa.PatientId == patient.PatientId)
                .Include(pa => pa.Allergy)
                .ToListAsync();

            var medications = await _context.PatientMedications
                .Where(pm => pm.PatientId == patient.PatientId)
                .Include(pm => pm.Medication)
                .ToListAsync();

            ViewBag.PatientName = patient.Name + " " + patient.Surname;
            ViewBag.Conditions = conditions;
            ViewBag.Allergies = allergies;
            ViewBag.Medications = medications;

            return View();
        }

        // ============================================================
        // CONDITIONS
        // ============================================================
        public async Task<IActionResult> Conditions()
        {
            var patient = await GetCurrentPatient();
            if (patient == null) return RedirectToAction("Login", "Account", new { area = "" });

            var conditions = await _context.PatientConditions
                .Where(pc => pc.PatientId == patient.PatientId)
                .Include(pc => pc.MedicalCondition)
                .ToListAsync();

            ViewBag.PatientName = patient.Name + " " + patient.Surname;
            ViewBag.Conditions = conditions;
            return View();
        }

        // ============================================================
        // ALLERGIES
        // ============================================================
        public async Task<IActionResult> Allergies()
        {
            var patient = await GetCurrentPatient();
            if (patient == null) return RedirectToAction("Login", "Account", new { area = "" });

            var allergies = await _context.PatientAllergies
                .Where(pa => pa.PatientId == patient.PatientId)
                .Include(pa => pa.Allergy)
                .ToListAsync();

            ViewBag.PatientName = patient.Name + " " + patient.Surname;
            ViewBag.Allergies = allergies;
            return View();
        }

        // ============================================================
        // MEDICATIONS
        // ============================================================
        public async Task<IActionResult> Medications()
        {
            var patient = await GetCurrentPatient();
            if (patient == null) return RedirectToAction("Login", "Account", new { area = "" });

            var medications = await _context.PatientMedications
                .Where(pm => pm.PatientId == patient.PatientId)
                .Include(pm => pm.Medication)
                .ToListAsync();

            ViewBag.PatientName = patient.Name + " " + patient.Surname;
            ViewBag.Medications = medications;
            return View();
        }

        // ============================================================
        // DOCTOR ACCESS
        // ============================================================
        public async Task<IActionResult> DoctorAccess()
        {
            var patient = await GetCurrentPatient();
            if (patient == null) return RedirectToAction("Login", "Account", new { area = "" });

            var consents = await _context.PatientDoctorConsents
                .Where(c => c.PatientId == patient.PatientId)
                .Include(c => c.ConsentRequestItems)
                    .ThenInclude(cri => cri.TestRequest)
                .ToListAsync();

            var allUsers = await _context.Users.ToListAsync();
            var userMap = allUsers.ToDictionary(
                u => u.Id,
                u => ((u.FirstName ?? "") + " " + (u.LastName ?? "")).Trim()
            );

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

            var alreadyGranted = consents
                .Where(c => c.IsActive)
                .Select(c => c.DoctorUserId)
                .Where(id => id != null)
                .ToHashSet();

            var availableDoctors = new List<(string Id, string Name)>();
            foreach (var docId in doctorUserIds)
            {
                if (!alreadyGranted.Contains(docId) && userMap.ContainsKey(docId))
                {
                    availableDoctors.Add((docId, userMap[docId]));
                }
            }

            var testRequests = await _context.TestRequests
                .Where(r => r.PatientId == patient.UserId)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            ViewBag.PatientName = patient.Name + " " + patient.Surname;
            ViewBag.Consents = consents;
            ViewBag.UserMap = userMap;
            ViewBag.Doctors = availableDoctors;
            ViewBag.TestRequests = testRequests;

            return View();
        }

        // ============================================================
        // GRANT CONSENT
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> GrantConsent(string doctorUserId, List<int> selectedRequestIds)
        {
            var patient = await GetCurrentPatient();
            if (patient == null) return RedirectToAction("Login", "Account", new { area = "" });

            var existing = await _context.PatientDoctorConsents
                .FirstOrDefaultAsync(c => c.PatientId == patient.PatientId
                                       && c.DoctorUserId == doctorUserId
                                       && c.IsActive);

            if (existing != null)
            {
                TempData["Error"] = "You have already granted this doctor access.";
                return RedirectToAction("DoctorAccess");
            }

            var consent = new PatientDoctorConsentModel
            {
                PatientId = patient.PatientId,
                DoctorUserId = doctorUserId,
                GrantedDate = DateTime.Now,
                IsActive = true,
                Notes = "Consent granted by patient"
            };

            _context.PatientDoctorConsents.Add(consent);
            await _context.SaveChangesAsync();

            if (selectedRequestIds != null && selectedRequestIds.Any())
            {
                foreach (var requestId in selectedRequestIds)
                {
                    _context.ConsentRequestItems.Add(new ConsentRequestItemModel
                    {
                        PatientConsentId = consent.PatientDoctorConsentId,
                        TestRequestId = requestId
                    });
                }
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Access granted successfully.";
            return RedirectToAction("DoctorAccess");
        }

        // ============================================================
        // REVOKE ALL CONSENT
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> RevokeConsent(int consentId)
        {
            var patient = await GetCurrentPatient();
            if (patient == null) return RedirectToAction("Login", "Account", new { area = "" });

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

            return RedirectToAction("DoctorAccess");
        }

        // ============================================================
        // REVOKE SINGLE RECORD
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> RevokeConsentItem(int itemId)
        {
            var patient = await GetCurrentPatient();
            if (patient == null) return RedirectToAction("Login", "Account", new { area = "" });

            var item = await _context.ConsentRequestItems
                .Include(i => i.PatientDoctorConsent)
                .FirstOrDefaultAsync(i => i.ConsentRequestItemId == itemId
                                       && i.PatientDoctorConsent.PatientId == patient.PatientId);

            if (item != null)
            {
                _context.ConsentRequestItems.Remove(item);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Record access revoked.";
            }

            return RedirectToAction("DoctorAccess");
        }

        // ============================================================
        // PROFILE — GET
        // ============================================================
        public async Task<IActionResult> Profile()
        {
            var patient = await GetCurrentPatient();
            if (patient == null) return RedirectToAction("Login", "Account", new { area = "" });
            ViewBag.PatientName = patient.Name + " " + patient.Surname;
            return View(patient);
        }

        // ============================================================
        // PROFILE — UPDATE (phone / address / email)
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(string contactNumber, string address, string email)
        {
            var patient = await GetCurrentPatient();
            if (patient == null) return RedirectToAction("Login", "Account", new { area = "" });

            if (!string.IsNullOrWhiteSpace(contactNumber))
                patient.ContactNumber = contactNumber.Trim();

            if (!string.IsNullOrWhiteSpace(address))
                patient.Address = address.Trim();

            if (!string.IsNullOrWhiteSpace(email))
            {
                // Check email is not already used by another patient
                var clash = await _context.Patients
                    .FirstOrDefaultAsync(p => p.Email == email.Trim() && p.PatientId != patient.PatientId);
                if (clash != null)
                {
                    TempData["Error"] = "That email is already in use.";
                    return RedirectToAction("Profile");
                }
                patient.Email = email.Trim();
            }

            patient.LastUpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction("Profile");
        }

        // ============================================================
        // CHANGE PASSWORD — GET (page)
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var patient = await GetCurrentPatient();
            if (patient == null) return RedirectToAction("Login", "Account", new { area = "" });
            ViewBag.PatientName = patient.Name + " " + patient.Surname;
            return View();
        }

        // ============================================================
        // CHANGE PASSWORD — POST
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(
            string currentPassword,
            string newPassword,
            string confirmPassword)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return RedirectToAction("Login", "Account", new { area = "" });

            // Verify current password
            if (user.PasswordHash != HashPassword(currentPassword))
            {
                TempData["Error"] = "Current password is incorrect.";
                return RedirectToAction("ChangePassword");
            }

            // Validate new password rules
            var ok = !string.IsNullOrEmpty(newPassword)
                     && newPassword.Length >= 8
                     && newPassword.Any(char.IsUpper)
                     && newPassword.Any(char.IsDigit)
                     && newPassword.Any(c => !char.IsLetterOrDigit(c));

            if (!ok)
            {
                TempData["Error"] = "New password must be at least 8 characters with one uppercase, one number and one special character.";
                return RedirectToAction("ChangePassword");
            }

            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "New passwords do not match.";
                return RedirectToAction("ChangePassword");
            }

            if (newPassword == currentPassword)
            {
                TempData["Error"] = "New password must be different from the current password.";
                return RedirectToAction("ChangePassword");
            }

            user.PasswordHash = HashPassword(newPassword);
            user.MustChangePassword = false;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Password updated successfully.";
            return RedirectToAction("ChangePassword");
        }
    }
}