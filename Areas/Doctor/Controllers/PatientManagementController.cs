using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NMB_HLabSys.Areas.Patient.Models;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Account;
using Microsoft.EntityFrameworkCore;
using PatientModel = NMB_HLabSys.Areas.Administrator.Models.Patient;
using Microsoft.AspNetCore.Identity.UI.Services;
using NMB_HLabSys.ViewModels.Doctor;

namespace NMB_HLabSys.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    public class PatientManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<RegisterUser> _userManager;
        private readonly IEmailSender _emailSender;

        public PatientManagementController(
            ApplicationDbContext context,
            UserManager<RegisterUser> userManager,
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        private async Task<int> GetCurrentDoctorIdAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return 0;

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.UserID == userId);

            return doctor?.DoctorId ?? 0;
        }

        // GET: /PatientManagement/Index
        public async Task<IActionResult> Index()
        {
            var doctorId = await GetCurrentDoctorIdAsync();
            if (doctorId == 0)
                return View(new List<PatientModel>());

            var patients = await _context.Patients
                .Include(p => p.RegisterUser)
                .Where(p => p.PatientDoctorConsents
                    .Any(c => c.DoctorUserId == doctorId.ToString() && c.IsActive))
                .ToListAsync();

            return View(patients);
        }

        // GET: /PatientManagement/Search
        public async Task<IActionResult> Search(string saIdNumber)
        {
            if (string.IsNullOrEmpty(saIdNumber)) return View();

            var patient = await _context.Patients
                .Include(p => p.PatientConditions)
                    .ThenInclude(pc => pc.MedicalCondition)
                .Include(p => p.PatientAllergies)
                    .ThenInclude(pa => pa.Allergy)
                .Include(p => p.PatientMedications)
                    .ThenInclude(pm => pm.Medication)
                .FirstOrDefaultAsync(p => p.IDNumber == saIdNumber);

            ViewBag.SearchId = saIdNumber;
            return View(patient);
        }

        // Helper to populate dropdowns
        private async Task PopulateViewModel(CreatePatientViewModel vm)
        {
            vm.AvailableConditions = await _context.MedicalConditions
                .Select(c => new SelectListItem
                {
                    Value = c.MedicalConditionId.ToString(),
                    Text = c.ConditionName
                }).ToListAsync();

            vm.AvailableAllergies = await _context.Allergies
                .Select(a => new SelectListItem
                {
                    Value = a.AllergyId.ToString(),
                    Text = a.AllergyName
                }).ToListAsync();

            vm.AvailableMedications = await _context.Medications
                .Select(m => new SelectListItem
                {
                    Value = m.MedicationId.ToString(),
                    Text = m.MedicationName
                }).ToListAsync();
        }

        // GET: /PatientManagement/Create
        public async Task<IActionResult> Create()
        {
            var vm = new CreatePatientViewModel();
            await PopulateViewModel(vm);
            return View(vm);
        }

        // POST: /PatientManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePatientViewModel vm)
        {
            // Remove navigation/dropdown properties from validation
            ModelState.Remove(nameof(vm.AvailableConditions));
            ModelState.Remove(nameof(vm.AvailableAllergies));
            ModelState.Remove(nameof(vm.AvailableMedications));

            if (!ModelState.IsValid)
            {
                await PopulateViewModel(vm);
                return View(vm);
            }

            // Check SA ID not already registered
            var exists = await _context.Patients
                .AnyAsync(p => p.IDNumber == vm.IDNumber);
            if (exists)
            {
                ModelState.AddModelError("IDNumber", "A patient with this ID number already exists.");
                await PopulateViewModel(vm);
                return View(vm);
            }

            // Generate temp password
            var tempPassword = GenerateTempPassword();

            // Create Identity account
            var user = new RegisterUser
            {
                UserName = vm.Email,
                Email = vm.Email,
                FullName = vm.Name + " " + vm.Surname,
                //IsFirstLogin = true,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, tempPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                await PopulateViewModel(vm);
                return View(vm);
            }

            await _userManager.AddToRoleAsync(user, "Patient");

            // Create Patient record
            var patient = new PatientModel
            {
                Name = vm.Name,
                Surname = vm.Surname,
                IDNumber = vm.IDNumber,
                DateOfBirth = vm.DateOfBirth ?? DateTime.Now,
                CellPhone = vm.CellPhone,
                Email = vm.Email,
                HomeAddress = vm.HomeAddress,
                RegisterUserId = user.Id
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            // Add medical history
            foreach (var id in vm.SelectedConditionIds)
                _context.PatientConditions.Add(new PatientCondition
                {
                    PatientID = patient.PatientID,
                    MedicalConditionID = id
                });

            foreach (var id in vm.SelectedAllergyIds)
                _context.PatientAllergies.Add(new PatientAllergy
                {
                    PatientID = patient.PatientID,
                    AllergyID = id
                });

            foreach (var id in vm.SelectedMedicationIds)
                _context.PatientMedications.Add(new PatientMedication
                {
                    PatientID = patient.PatientID,
                    MedicationId = id
                });

            await _context.SaveChangesAsync();

            // Email patient their temp password
            try
            {
                await _emailSender.SendEmailAsync(vm.Email,
                    "Welcome to NMB-HLabSys",
                    $"Hello {vm.Name},<br/><br/>" +
                    $"Your patient account has been created.<br/>" +
                    $"<strong>Username:</strong> {vm.Email}<br/>" +
                    $"<strong>Temporary Password:</strong> {tempPassword}<br/><br/>" +
                    $"You will be required to change your password on first login.<br/><br/>" +
                    $"Regards,<br/>NMB HLabSys");
            }
            catch
            {
                // Email failure should not block patient creation
                TempData["Warning"] = $"Patient created but email could not be sent. Temporary password: {tempPassword}";
            }

            TempData["Success"] = $"Patient {vm.Name} {vm.Surname} registered successfully. Credentials emailed to {vm.Email}.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /PatientManagement/EditMedicalHistory/5
        public async Task<IActionResult> EditMedicalHistory(int id)
        {
            var patient = await _context.Patients
                .Include(p => p.PatientConditions)
                .Include(p => p.PatientAllergies)
                .Include(p => p.PatientMedications)
                .FirstOrDefaultAsync(p => p.PatientID == id);

            if (patient == null) return NotFound();

            var vm = new CreatePatientViewModel
            {
                Name = patient.Name,
                Surname = patient.Surname,
                IDNumber = patient.IDNumber,
                DateOfBirth = patient.DateOfBirth,
                CellPhone = patient.CellPhone,
                Email = patient.Email ?? string.Empty,
                HomeAddress = patient.HomeAddress,
                SelectedConditionIds = patient.PatientConditions
                    .Select(pc => pc.MedicalConditionID).ToList(),
                SelectedAllergyIds = patient.PatientAllergies
                    .Select(pa => pa.AllergyID).ToList(),
                SelectedMedicationIds = patient.PatientMedications
                    .Select(pm => pm.MedicationId).ToList()
            };

            await PopulateViewModel(vm);
            return View(vm);
        }

        // POST: /PatientManagement/EditMedicalHistory/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMedicalHistory(int id, CreatePatientViewModel vm)
        {
            ModelState.Remove(nameof(vm.AvailableConditions));
            ModelState.Remove(nameof(vm.AvailableAllergies));
            ModelState.Remove(nameof(vm.AvailableMedications));

            var patient = await _context.Patients
                .Include(p => p.PatientConditions)
                .Include(p => p.PatientAllergies)
                .Include(p => p.PatientMedications)
                .FirstOrDefaultAsync(p => p.PatientID == id);

            if (patient == null) return NotFound();

            // Remove old history
            _context.PatientConditions.RemoveRange(patient.PatientConditions);
            _context.PatientAllergies.RemoveRange(patient.PatientAllergies);
            _context.PatientMedications.RemoveRange(patient.PatientMedications);
            await _context.SaveChangesAsync();

            // Add updated history
            foreach (var condId in vm.SelectedConditionIds)
                _context.PatientConditions.Add(new PatientCondition
                {
                    PatientID = patient.PatientID,
                    MedicalConditionID = condId
                });

            foreach (var allergyId in vm.SelectedAllergyIds)
                _context.PatientAllergies.Add(new PatientAllergy
                {
                    PatientID = patient.PatientID,
                    AllergyID = allergyId
                });

            foreach (var medId in vm.SelectedMedicationIds)
                _context.PatientMedications.Add(new PatientMedication
                {
                    PatientID = patient.PatientID,
                    MedicationId = medId
                });

            await _context.SaveChangesAsync();

            TempData["Success"] = "Medical history updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // Helper: generate temp password meeting spec requirements
        private string GenerateTempPassword()
        {
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string special = "!@#$%^&*";
            var rng = new Random();

            var password = new[]
            {
                upper[rng.Next(upper.Length)].ToString(),
                digits[rng.Next(digits.Length)].ToString(),
                special[rng.Next(special.Length)].ToString(),
                lower[rng.Next(lower.Length)].ToString(),
                lower[rng.Next(lower.Length)].ToString(),
                lower[rng.Next(lower.Length)].ToString(),
                upper[rng.Next(upper.Length)].ToString(),
                digits[rng.Next(digits.Length)].ToString(),
            };

            return string.Concat(password.OrderBy(_ => rng.Next()));
        }
    }
}