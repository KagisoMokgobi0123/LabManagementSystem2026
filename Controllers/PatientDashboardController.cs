using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Administrator.Models;
using NMB_HLabSys.Areas.Patient.Models;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Account;
using NMB_HLabSys.Models.Identity;
using System.Security.Claims;

namespace NMB_HLabSys.Controllers
{
    [Authorize(Roles = "Patient")]
    public class PatientDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PatientDashboardController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Get logged-in user ID from Identity
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("LoginUser", "Account");

            // Get patient from database
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
            {
                // Create patient if doesn't exist
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return RedirectToAction("LoginUser", "Account");

                patient = new PatientModel
                {
                    PatientId = Guid.NewGuid().ToString(),
                    UserId = userId,
                    Name = user.FullName?.Split(' ').FirstOrDefault() ?? "Patient",
                    Surname = user.FullName?.Split(' ').Skip(1).FirstOrDefault() ?? "",
                    IDNumber = "0000000000000",
                    DateOfBirth = DateTime.Now.AddYears(-30),
                    ContactNumber = "0000000000",
                    Email = user.Email ?? "",
                    Address = "No address provided",
                    CreatedDate = DateTime.Now
                };

                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
            }

            // Get test requests
            var testRequests = await _context.TestRequests
                .Where(r => r.PatientId == patient.PatientId)
                .OrderByDescending(r => r.RequestDate)
                .Take(10)
                .ToListAsync();

            // Get all requests for stats
            var allRequests = await _context.TestRequests
                .Where(r => r.PatientId == patient.PatientId)
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
                RecentRequests = testRequests
            };

            ViewBag.UserName = patient.Name + " " + patient.Surname;
            ViewBag.UserEmail = patient.Email;

            return View(model);
        }
        public async Task<IActionResult> GetTestRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
                return NotFound();

            var requests = await _context.TestRequests
                .Where(r => r.PatientId == patient.PatientId)
                .OrderByDescending(r => r.RequestDate)
                .Select(r => new
                {
                    r.TestRequestId,
                    r.RequestNumber,
                    r.RequestDate,
                    r.Status,
                    r.Urgency,
                    r.ClinicalNotes,
                    TestItems = r.TestTypes//_context.TestRequestItems
                        //.Where(ti => ti.TestRequestId == r.TestRequestId)//KG changed from THIS to TestTypes
                        //.Select(ti => new
                        //{
                        //    ti.TestRequestItemId,
                        //    ti.TestTypeID,
                        //    ti.ResultValue,
                        //    ti.Status,
                        //    ti.Notes
                        //}).ToList()
                })
                .ToListAsync();

            return Json(new { success = true, data = requests });
        }

        public async Task<IActionResult> GetMedicalHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
                return NotFound();

            // ✅ FIXED: Use ConditionName instead of Name
            var conditions = await _context.PatientConditions
                .Where(pc => pc.PatientId == patient.PatientId)
                .Include(pc => pc.MedicalCondition)
                .Select(pc => new
                {
                    Name = pc.MedicalCondition != null ? pc.MedicalCondition.ConditionName : "Unknown",
                    pc.DateDiagnosed,
                    pc.Notes
                })
                .ToListAsync();

            // ✅ FIXED: Use AllergyName instead of Name
            var allergies = await _context.PatientAllergies
                .Where(pa => pa.PatientId == patient.PatientId)
                .Include(pa => pa.Allergy)
                .Select(pa => new
                {
                    Name = pa.Allergy != null ? pa.Allergy.AllergyName : "Unknown",
                    pa.Severity,
                    pa.Notes
                })
                .ToListAsync();

            // ✅ FIXED: Use MedicationName instead of Name
            var medications = await _context.PatientMedications
                .Where(pm => pm.PatientId == patient.PatientId)
                .Include(pm => pm.Medication)
                .Select(pm => new
                {
                    Name = pm.Medication != null ? pm.Medication.MedicationName : "Unknown",
                    pm.Dosage,
                    pm.Frequency,
                    pm.StartDate,
                    pm.EndDate
                })
                .ToListAsync();

            return Json(new
            {
                success = true,
                conditions,
                allergies,
                medications
            });
        }
      
    // ===== TEST REQUESTS =====
        public async Task<IActionResult> TestRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return RedirectToAction("Index");

            var requests = await _context.TestRequests
                .Where(r => r.PatientId == patient.PatientId)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            ViewBag.PatientName = patient.Name + " " + patient.Surname;
            return View(requests);
        }

        // ===== TEST RESULTS =====
        public async Task<IActionResult> TestResults()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return RedirectToAction("Index");

            var results = await _context.TestRequests
                .Where(r => r.PatientId == patient.PatientId &&
                           (r.Status == "Completed" || r.Status == "ReleasedByDoctor"))
                .OrderByDescending(r => r.RequestDate)
                .Include(r => r.TestTypes)//KG changed from TestRequestItem to TestTypes
                .ToListAsync();

            ViewBag.PatientName = patient.Name + " " + patient.Surname;
            return View(results);
        }
        // ===== TRACK RESULTS OVER TIME =====
        public async Task<IActionResult> TrackResults()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("LoginUser", "Account");

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
                return RedirectToAction("Index");

            var results = await _context.TestRequests
                .Where(r => r.PatientId == patient.PatientId &&
                            (r.Status == "Completed" ||
                             r.Status == "ReleasedByDoctor"))
                .OrderByDescending(r => r.RequestDate)
                .Include(r => r.TestTypes)//KG changed from TestRequestItem to TestTypes
                .ToListAsync();

            ViewBag.PatientName = patient.Name + " " + patient.Surname;

            return View(results);
        }

        // ===== MEDICAL HISTORY PAGE =====
        public async Task<IActionResult> MedicalHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return RedirectToAction("Index");

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

        // ===== CONSENT MANAGEMENT =====
        public async Task<IActionResult> Consent()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return RedirectToAction("Index");

            var consents = await _context.PatientDoctorConsents
                .Where(c => c.PatientId == patient.PatientId)
                .Include(c => c.Doctor)
                .Include(c => c.ConsentRequestItems)
                    .ThenInclude(cri => cri.TestRequest)
                .ToListAsync();

            // Get all doctors for the grant consent dropdown
            // Get only users in the Doctor role
            var doctorRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == "Doctor");

            var doctorUserIds = await _context.UserRoles
                .Where(ur => ur.RoleId == doctorRole.Id)
                .Select(ur => ur.UserId)
                .ToListAsync();

            // Exclude doctors already granted consent
            var alreadyGranted = await _context.PatientDoctorConsents
                .Where(c => c.PatientId == patient.PatientId && c.IsActive)
                .Select(c => c.DoctorUserId)
                .ToListAsync();

            var doctors = await _context.Users
                .Where(u => doctorUserIds.Contains(u.Id) && !alreadyGranted.Contains(u.Id))
                .ToListAsync();

            // Get patient test requests for consent selection
            var testRequests = await _context.TestRequests
                .Where(r => r.PatientId == patient.PatientId)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            ViewBag.PatientName = patient.Name + " " + patient.Surname;
            ViewBag.PatientId = patient.PatientId;
            ViewBag.Consents = consents;
            ViewBag.Doctors = doctors;
            ViewBag.TestRequests = testRequests;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GrantConsent(string doctorUserId, List<int> selectedRequestIds)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return RedirectToAction("Index");

            // Check if consent already exists
            var existing = await _context.PatientDoctorConsents
                .FirstOrDefaultAsync(c => c.PatientId == patient.PatientId
                                       && c.DoctorUserId == doctorUserId
                                       && c.IsActive);

            if (existing != null)
            {
                TempData["Error"] = "You have already granted this doctor access.";
                return RedirectToAction("Consent");
            }

            // Create consent
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

            // Add selected test requests
            if (selectedRequestIds != null && selectedRequestIds.Any())
            {
                foreach (var requestId in selectedRequestIds)
                {
                    var item = new ConsentRequestItemModel
                    {
                        PatientConsentId = consent.PatientDoctorConsentId,
                        TestRequestId = requestId
                    };
                    _context.ConsentRequestItems.Add(item);
                }
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Access granted successfully.";
            return RedirectToAction("Consent");
        }

        [HttpPost]
        public async Task<IActionResult> RevokeConsent(int consentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return RedirectToAction("Index");

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

            return RedirectToAction("Consent");
        }

        [HttpPost]
        public async Task<IActionResult> RevokeConsentItem(int itemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return RedirectToAction("Index");

            var item = await _context.ConsentRequestItems
                .Include(i => i.PatientDoctorConsent)
                .FirstOrDefaultAsync(i => i.ConsentRequestItemId == itemId
                                       && i.PatientDoctorConsent.PatientId == patient.PatientId);

            if (item != null)
            {
                _context.ConsentRequestItems.Remove(item);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Record access revoked successfully.";
            }

            return RedirectToAction("Consent");
        }

        // ===== PROFILE =====
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return RedirectToAction("Index");

            ViewBag.PatientName = patient.Name + " " + patient.Surname;
            return View(patient);
        }
        // ===== UPDATE PROFILE =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(
            string ContactNumber,
            string Email,
            string Address)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("LoginUser", "Account");

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
                return RedirectToAction("Index");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return RedirectToAction("LoginUser", "Account");

            // Basic validation
            if (string.IsNullOrWhiteSpace(ContactNumber))
            {
                ModelState.AddModelError("ContactNumber", "Cellphone number is required.");
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                ModelState.AddModelError("Email", "Email address is required.");
            }
            else if (!new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(Email))
            {
                ModelState.AddModelError("Email", "Please enter a valid email address.");
            }

            if (string.IsNullOrWhiteSpace(Address))
            {
                ModelState.AddModelError("Address", "Home address is required.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.PatientName = patient.Name + " " + patient.Surname;
                return View("Profile", patient);
            }

            // Update Patient record
            patient.ContactNumber = ContactNumber.Trim();
            patient.Email = Email.Trim();
            patient.Address = Address.Trim();
            patient.LastUpdatedDate = DateTime.Now;

            // Keep Identity account synchronized
            user.Email = Email.Trim();
            user.UserName = Email.Trim();
            user.NormalizedEmail = Email.Trim().ToUpperInvariant();
            user.NormalizedUserName = Email.Trim().ToUpperInvariant();

            // Update Identity user
            var identityResult = await _userManager.UpdateAsync(user);

            if (!identityResult.Succeeded)
            {
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                ViewBag.PatientName = patient.Name + " " + patient.Surname;
                return View("Profile", patient);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Your profile has been updated successfully.";

            return RedirectToAction("Profile");
        }
        // ===== TRACK RESULTS OVER TIME =====
        //public async Task<IActionResult> TrackResults()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    var patient = await _context.Patients
        //        .FirstOrDefaultAsync(p => p.UserId == userId);

        //    if (patient == null)
        //        return RedirectToAction("Index");

        //    var results = await _context.TestRequests
        //        .Where(r => r.PatientId == patient.PatientId &&
        //                    (r.Status == "Completed" ||
        //                     r.Status == "ReleasedByDoctor"))
        //        .OrderBy(r => r.RequestDate)
        //        .Include(r => r.TestRequestItems)
        //        .ToListAsync();

        //    ViewBag.PatientName = patient.Name + " " + patient.Surname;

        //    return View(results);
        //}

        // ===== PATIENT REPORTS =====
        public async Task<IActionResult> PatientReports()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("LoginUser", "Account");

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
                return RedirectToAction("Index");

            var reports = await _context.TestRequests
                .Where(r => r.PatientId == patient.PatientId &&
                            (r.Status == "Completed" ||
                             r.Status == "ReleasedByDoctor"))
                .OrderByDescending(r => r.RequestDate)
                .Include(r => r.TestTypes)//KG changed from TestRequestItem to TestTypes
                .ToListAsync();

            ViewBag.PatientName = patient.Name + " " + patient.Surname;
            ViewBag.PatientID = patient.IDNumber;

            return View(reports);
        }

    }
} 