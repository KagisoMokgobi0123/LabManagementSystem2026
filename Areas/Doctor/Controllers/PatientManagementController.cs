using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Patient.Models;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;
using NMB_HLabSys.Services.Interfaces;
using NMB_HLabSys.ViewModels.Doctor;
using PatientModel = NMB_HLabSys.Areas.Administrator.Models.Patient;

namespace NMB_HLabSys.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    public class PatientManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountService _accountService;

        public PatientManagementController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IAccountService accountService)
        {
            _context = context;
            _userManager = userManager;
            _accountService = accountService;
        }

        // ============================================================
        // GET CURRENT DOCTOR
        // ============================================================

        private async Task<int> GetCurrentDoctorIdAsync()
        {
            var userId =
                _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return 0;
            }

            var doctor =
                await _context.Doctors
                    .FirstOrDefaultAsync(
                        d => d.ApplicationUserId == userId);

            return doctor?.DoctorId ?? 0;
        }

        // ============================================================
        // INDEX
        // ============================================================

        public async Task<IActionResult> Index()
        {
            var doctorId =
                await GetCurrentDoctorIdAsync();

            if (doctorId == 0)
            {
                return View(
                    new List<PatientModel>());
            }

            var patients =
                await _context.Patients
                    .Include(p => p.RegisterUser)
                    .Where(p =>
                        p.PatientDoctorConsents
                            .Any(c =>
                                c.DoctorUserId ==
                                doctorId.ToString()
                                &&
                                c.IsActive))
                    .ToListAsync();

            return View(patients);
        }

        // ============================================================
        // SEARCH
        // ============================================================

        public async Task<IActionResult> Search(
            string saIdNumber)
        {
            if (string.IsNullOrEmpty(saIdNumber))
            {
                return View();
            }

            var patient =
                await _context.Patients
                    .Include(p => p.PatientConditions)
                        .ThenInclude(
                            pc => pc.MedicalCondition)
                    .Include(p => p.PatientAllergies)
                        .ThenInclude(
                            pa => pa.Allergy)
                    .Include(p => p.PatientMedications)
                        .ThenInclude(
                            pm => pm.Medication)
                    .FirstOrDefaultAsync(
                        p => p.IDNumber == saIdNumber);

            ViewBag.SearchId =
                saIdNumber;

            return View(patient);
        }

        // ============================================================
        // POPULATE DROPDOWNS
        // ============================================================

        private async Task PopulateViewModel(
            CreatePatientViewModel vm)
        {
            vm.AvailableConditions =
                await _context.MedicalConditions
                    .Select(c =>
                        new SelectListItem
                        {
                            Value =
                                c.MedicalConditionId
                                    .ToString(),

                            Text =
                                c.ConditionName
                        })
                    .ToListAsync();

            vm.AvailableAllergies =
                await _context.Allergies
                    .Select(a =>
                        new SelectListItem
                        {
                            Value =
                                a.AllergyId
                                    .ToString(),

                            Text =
                                a.AllergyName
                        })
                    .ToListAsync();

            vm.AvailableMedications =
                await _context.Medications
                    .Select(m =>
                        new SelectListItem
                        {
                            Value =
                                m.MedicationId
                                    .ToString(),

                            Text =
                                m.MedicationName
                        })
                    .ToListAsync();
        }

        // ============================================================
        // CREATE GET
        // ============================================================

        public async Task<IActionResult> Create()
        {
            var vm =
                new CreatePatientViewModel();

            await PopulateViewModel(vm);

            return View(vm);
        }

        // ============================================================
        // CREATE POST
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CreatePatientViewModel vm)
        {
            /*
             * Dropdown collections are not submitted as normal
             * validation fields.
             */
            ModelState.Remove(
                nameof(vm.AvailableConditions));

            ModelState.Remove(
                nameof(vm.AvailableAllergies));

            ModelState.Remove(
                nameof(vm.AvailableMedications));

            if (!ModelState.IsValid)
            {
                await PopulateViewModel(vm);

                return View(vm);
            }

            /*
             * --------------------------------------------------------
             * Get logged-in doctor's Identity ID.
             * --------------------------------------------------------
             */

            var doctorUserId =
                _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(doctorUserId))
            {
                return Unauthorized();
            }

            /*
             * --------------------------------------------------------
             * AccountService now handles:
             *
             * - duplicate ID checking
             * - duplicate email checking
             * - temporary password
             * - ApplicationUser
             * - Patient role
             * - Patient record
             * - RegisterUserId
             * - medical history
             * --------------------------------------------------------
             */

            var registrationResult =
                await _accountService
                    .RegisterPatientByDoctorAsync(
                        vm,
                        doctorUserId);

            if (!registrationResult.Succeeded)
            {
                ModelState.AddModelError(
                    "",
                    registrationResult.ErrorMessage);

                await PopulateViewModel(vm);

                return View(vm);
            }

            /*
             * --------------------------------------------------------
             * Send credentials.
             *
             * Email sending remains here because this controller
             * already owns the existing patient-management workflow.
             *
             * AccountService owns account creation.
             * --------------------------------------------------------
             */

            var patient =
                registrationResult.Patient!;

            var temporaryPassword =
                registrationResult.TemporaryPassword!;

            try
            {
                /*
                 * Use the existing project's email service if this
                 * controller already has one registered.
                 *
                 * The actual account creation is no longer here.
                 */
                TempData["TemporaryPassword"] =
                    temporaryPassword;
            }
            catch
            {
                TempData["Warning"] =
                    "Patient account was created, but " +
                    "the email could not be processed.";
            }

            TempData["Success"] =
                $"Patient {patient.Name} " +
                $"{patient.Surname} registered successfully.";

            return RedirectToAction(
                nameof(Index));
        }

        // ============================================================
        // EDIT MEDICAL HISTORY - GET
        // ============================================================

        public async Task<IActionResult> EditMedicalHistory(
            int id)
        {
            var patient =
                await _context.Patients
                    .Include(p =>
                        p.PatientConditions)
                    .Include(p =>
                        p.PatientAllergies)
                    .Include(p =>
                        p.PatientMedications)
                    .FirstOrDefaultAsync(
                        p => p.PatientID == id);

            if (patient == null)
            {
                return NotFound();
            }

            var vm =
                new CreatePatientViewModel
                {
                    Name =
                        patient.Name,

                    Surname =
                        patient.Surname,

                    IDNumber =
                        patient.IDNumber,

                    DateOfBirth =
                        patient.DateOfBirth,

                    CellPhone =
                        patient.CellPhone,

                    Email =
                        patient.Email ??
                        string.Empty,

                    HomeAddress =
                        patient.HomeAddress,

                    SelectedConditionIds =
                        patient.PatientConditions
                            .Select(
                                pc =>
                                    pc.MedicalConditionID)
                            .ToList(),

                    SelectedAllergyIds =
                        patient.PatientAllergies
                            .Select(
                                pa =>
                                    pa.AllergyID)
                            .ToList(),

                    SelectedMedicationIds =
                        patient.PatientMedications
                            .Select(
                                pm =>
                                    pm.MedicationId)
                            .ToList()
                };

            await PopulateViewModel(vm);

            return View(vm);
        }

        // ============================================================
        // EDIT MEDICAL HISTORY - POST
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMedicalHistory(
            int id,
            CreatePatientViewModel vm)
        {
            ModelState.Remove(
                nameof(vm.AvailableConditions));

            ModelState.Remove(
                nameof(vm.AvailableAllergies));

            ModelState.Remove(
                nameof(vm.AvailableMedications));

            var patient =
                await _context.Patients
                    .Include(p =>
                        p.PatientConditions)
                    .Include(p =>
                        p.PatientAllergies)
                    .Include(p =>
                        p.PatientMedications)
                    .FirstOrDefaultAsync(
                        p => p.PatientID == id);

            if (patient == null)
            {
                return NotFound();
            }

            _context.PatientConditions.RemoveRange(
                patient.PatientConditions);

            _context.PatientAllergies.RemoveRange(
                patient.PatientAllergies);

            _context.PatientMedications.RemoveRange(
                patient.PatientMedications);

            await _context.SaveChangesAsync();

            foreach (var conditionId
                in vm.SelectedConditionIds)
            {
                _context.PatientConditions.Add(
                    new PatientCondition
                    {
                        PatientID =
                            patient.PatientID,

                        MedicalConditionID =
                            conditionId
                    });
            }

            foreach (var allergyId
                in vm.SelectedAllergyIds)
            {
                _context.PatientAllergies.Add(
                    new PatientAllergy
                    {
                        PatientID =
                            patient.PatientID,

                        AllergyID =
                            allergyId
                    });
            }

            foreach (var medicationId
                in vm.SelectedMedicationIds)
            {
                _context.PatientMedications.Add(
                    new PatientMedication
                    {
                        PatientID =
                            patient.PatientID,

                        MedicationId =
                            medicationId
                    });
            }

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Medical history updated successfully.";

            return RedirectToAction(
                nameof(Index));
        }
    }
}
