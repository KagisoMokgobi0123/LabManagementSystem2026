using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Administrator.Models;
using NMB_HLabSys.Areas.Doctor.Services;
using NMB_HLabSys.Areas.Doctor.ViewModels;
using NMB_HLabSys.Areas.Patient.Models;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Areas.Doctor.Controllers;

[Area("Doctor"), Authorize(Roles = "Doctor")]
public sealed class PatientsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _users;
    private readonly RoleManager<IdentityRole> _roles;
    private readonly DoctorEmailService _email;
    private readonly ILogger<PatientsController> _logger;
    public PatientsController(ApplicationDbContext db, UserManager<ApplicationUser> users,
        RoleManager<IdentityRole> roles, DoctorEmailService email, ILogger<PatientsController> logger)
    { _db = db; _users = users; _roles = roles; _email = email; _logger = logger; }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm, string? idNumber)
    {
        // Doctors can browse existing patient records and narrow the list by ID,
        // name, surname, email, or contact number. Keep this endpoint role-protected.
        var query = _db.Patients
            .AsNoTracking()
            .Include(p => p.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(idNumber))
        {
            var cleanId = idNumber.Trim();
            query = query.Where(p => p.IDNumber == cleanId);
            searchTerm = cleanId;
        }
        else if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(p =>
                p.IDNumber.Contains(term) ||
                p.Name.Contains(term) ||
                p.Surname.Contains(term) ||
                p.Email.Contains(term) ||
                p.ContactNumber.Contains(term));
        }

        var patients = await query
            .OrderBy(p => p.Surname)
            .ThenBy(p => p.Name)
            .ToListAsync();

        ViewBag.SearchTerm = searchTerm;
        ViewBag.SearchId = idNumber;
        return View(patients);
    }

    /// <summary>
    /// AJAX endpoint used by the Doctor patient lookup page.
    /// The view calls SearchSuggestions?term=..., so the action name and JSON
    /// response shape must stay in sync with that JavaScript.
    /// </summary>
    [HttpGet]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> SearchSuggestions(
        string? term,
        CancellationToken cancellationToken)
    {
        term = (term ?? string.Empty).Trim();

        if (term.Length == 0 || term.Any(c => c < '0' || c > '9') || term.Length > 13)
        {
            return Json(new
            {
                requiresFullId = false,
                patients = Array.Empty<object>()
            });
        }

        if (term.Length < 13)
        {
            return Json(new
            {
                requiresFullId = true,
                patients = Array.Empty<object>()
            });
        }

        var matches = await _db.Patients
            .AsNoTracking()
            .Where(p => p.IDNumber == term)
            .Select(p => new
            {
                p.PatientId,
                p.Name,
                p.Surname,
                p.IDNumber,
                p.DateOfBirth
            })
            .Take(10)
            .ToListAsync(cancellationToken);

        var patients = matches.Select(p => new
        {
            name = string.Join(" ", new[] { p.Name, p.Surname }
                .Where(part => !string.IsNullOrWhiteSpace(part))),
            idNumber = p.IDNumber,
            dateOfBirth = p.DateOfBirth.ToString("dd MMM yyyy"),
            detailsUrl = Url.Action(nameof(Details), "Patients",
                new { area = "Doctor", id = p.PatientId })
        }).ToList();

        return Json(new
        {
            requiresFullId = false,
            patients
        });
    }

    [HttpGet]
    public IActionResult Search(string? idNumber) => RedirectToAction(nameof(Index), new { idNumber });

    [HttpGet]
    public async Task<IActionResult> Create(string? idNumber)
    {
        var model = new PatientCreateViewModel { IDNumber = idNumber?.Trim() ?? "" };
        if (NMB_HLabSys.Validation.SouthAfricanIdNumberAttribute.TryGetDateOfBirth(model.IDNumber) is DateTime dob)
            model.DateOfBirth = dob;
        await LoadCatalogsAsync();
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PatientCreateViewModel model)
    {
        var cleanId = (model.IDNumber ?? "").Trim();
        model.IDNumber = cleanId;

        var cleanContactNumber = (model.ContactNumber ?? "").Trim();
        model.ContactNumber = cleanContactNumber;
        if (cleanContactNumber.Length != 10 || cleanContactNumber[0] != '0' || cleanContactNumber.Any(c => c < '0' || c > '9'))
            ModelState.AddModelError(nameof(model.ContactNumber), "Enter a South African contact number containing exactly 10 digits and starting with 0.");

        var embeddedDob = NMB_HLabSys.Validation.SouthAfricanIdNumberAttribute.TryGetDateOfBirth(cleanId);
        if (embeddedDob == null) ModelState.AddModelError(nameof(model.IDNumber), "Enter a valid South African ID number, including a valid date and checksum.");
        else if (model.DateOfBirth?.Date != embeddedDob.Value.Date) ModelState.AddModelError(nameof(model.DateOfBirth), "Date of birth must match the date encoded in the ID number.");
        if (model.DateOfBirth?.Date > DateTime.Today) ModelState.AddModelError(nameof(model.DateOfBirth), "Date of birth cannot be in the future.");
        if (await _db.Patients.AnyAsync(p => p.IDNumber == cleanId)) ModelState.AddModelError(nameof(model.IDNumber), "A patient with this ID number already exists. Search for the patient instead.");
        if (!string.IsNullOrWhiteSpace(model.Email) && await _users.FindByEmailAsync(model.Email.Trim()) != null) ModelState.AddModelError(nameof(model.Email), "An account already exists with this email address.");

        model.MedicalConditionIds ??= new List<int>(); model.AllergyIds ??= new List<int>(); model.MedicationIds ??= new List<int>();
        if (model.MedicalConditionIds.Count == 0 && !model.NoKnownMedicalConditions) ModelState.AddModelError(nameof(model.MedicalConditionIds), "Select at least one medical condition or confirm that no known conditions were reported.");
        if (model.MedicalConditionIds.Count > 0 && model.NoKnownMedicalConditions) ModelState.AddModelError(nameof(model.NoKnownMedicalConditions), "Choose either listed conditions or 'No known conditions', not both.");
        if (model.AllergyIds.Count == 0 && !model.NoKnownAllergies) ModelState.AddModelError(nameof(model.AllergyIds), "Select at least one allergy or confirm that no known allergies were reported.");
        if (model.AllergyIds.Count > 0 && model.NoKnownAllergies) ModelState.AddModelError(nameof(model.NoKnownAllergies), "Choose either listed allergies or 'No known allergies', not both.");
        if (model.MedicationIds.Count == 0 && !model.NoCurrentMedications) ModelState.AddModelError(nameof(model.MedicationIds), "Select at least one medication or confirm that the patient has no current medications.");
        if (model.MedicationIds.Count > 0 && model.NoCurrentMedications) ModelState.AddModelError(nameof(model.NoCurrentMedications), "Choose either listed medications or 'No current medications', not both.");

        var conditionIds = await _db.MedicalConditions.Where(x => model.MedicalConditionIds.Contains(x.MedicalConditionId) && x.Status == NMB_HLabSys.Areas.Administrator.Models.Enums.Status.Active).Select(x => x.MedicalConditionId).ToListAsync();
        var allergyIds = await _db.Allergies.Where(x => model.AllergyIds.Contains(x.AllergyId) && x.Status == NMB_HLabSys.Areas.Administrator.Models.Enums.Status.Active).Select(x => x.AllergyId).ToListAsync();
        var medicationIds = await _db.Medications.Where(x => model.MedicationIds.Contains(x.MedicationId) && x.Status == NMB_HLabSys.Areas.Administrator.Models.Enums.Status.Active).Select(x => x.MedicationId).ToListAsync();
        if (conditionIds.Count != model.MedicalConditionIds.Distinct().Count()) ModelState.AddModelError(nameof(model.MedicalConditionIds), "One or more selected medical conditions are no longer available.");
        if (allergyIds.Count != model.AllergyIds.Distinct().Count()) ModelState.AddModelError(nameof(model.AllergyIds), "One or more selected allergies are no longer available.");
        if (medicationIds.Count != model.MedicationIds.Distinct().Count()) ModelState.AddModelError(nameof(model.MedicationIds), "One or more selected medications are no longer available.");
        if (!ModelState.IsValid) { await LoadCatalogsAsync(); return View(model); }

        var temporaryPassword = GenerateTemporaryPassword();
        var user = new ApplicationUser
        {
            UserName = model.Email.Trim(),
            Email = model.Email.Trim(),
            EmailConfirmed = true,
            FirstName = model.Name.Trim(),
            LastName = model.Surname.Trim(),
            PhoneNumber = model.ContactNumber.Trim(),
            IsActive = true,
            MustChangePassword = true,
            IsProfileCompleted = false,
            CreatedDate = DateTime.UtcNow
        };
        var created = await _users.CreateAsync(user, temporaryPassword);
        if (!created.Succeeded)
        {
            foreach (var e in created.Errors) ModelState.AddModelError("", e.Description);
            await LoadCatalogsAsync(); return View(model);
        }

        try
        {
            if (!await _roles.RoleExistsAsync("Patient"))
            {
                var roleResult = await _roles.CreateAsync(new IdentityRole("Patient"));
                if (!roleResult.Succeeded) throw new InvalidOperationException("Unable to configure the Patient role.");
            }
            var roleAdded = await _users.AddToRoleAsync(user, "Patient");
            if (!roleAdded.Succeeded) throw new InvalidOperationException(string.Join("; ", roleAdded.Errors.Select(x => x.Description)));

            var patient = new PatientModel
            {
                PatientId = Guid.NewGuid().ToString(),
                UserId = user.Id,
                Name = model.Name.Trim(),
                Surname = model.Surname.Trim(),
                IDNumber = cleanId,
                DateOfBirth = embeddedDob!.Value.Date,
                ContactNumber = model.ContactNumber.Trim(),
                Email = model.Email.Trim(),
                Address = model.Address.Trim(),
                Suburb = model.Suburb?.Trim() ?? "",
                City = model.City.Trim(),
                Province = model.Province.Trim(),
                PostalCode = model.PostalCode?.Trim() ?? "",
                CreatedDate = DateTime.Now,
                NoKnownMedicalConditions = model.NoKnownMedicalConditions,
                NoKnownAllergies = model.NoKnownAllergies,
                NoCurrentMedications = model.NoCurrentMedications
            };
            _db.Patients.Add(patient);
            await _db.SaveChangesAsync();

            // The administrator's catalogue is authoritative. Keep the existing patient-history catalog tables in sync by name.
            foreach (var id in conditionIds)
            {
                var source = await _db.MedicalConditions.FindAsync(id);
                if (source == null) continue;
                var target = await _db.Set<MedicalConditionModel>().FirstOrDefaultAsync(x => x.ConditionName == source.Name && x.MedicalConditionCategoryId == source.MedicalConditionCategoryId);
                if (target == null) { target = new MedicalConditionModel { ConditionName = source.Name, Description = source.Description, MedicalConditionCategoryId = source.MedicalConditionCategoryId }; _db.Add(target); await _db.SaveChangesAsync(); }
                _db.PatientConditions.Add(new PatientConditionModel { PatientId = patient.PatientId, MedicalConditionId = target.MedicalConditionId, DateDiagnosed = DateTime.Today });
            }
            foreach (var id in allergyIds)
            {
                var source = await _db.Allergies.FindAsync(id);
                if (source == null) continue;
                var target = await _db.Set<AllergyModel>().FirstOrDefaultAsync(x => x.AllergyName == source.Name && x.AllergyCategoryId == source.AllergyCategoryId);
                if (target == null) { target = new AllergyModel { AllergyName = source.Name, Description = source.Description, AllergyCategoryId = source.AllergyCategoryId }; _db.Add(target); await _db.SaveChangesAsync(); }
                _db.PatientAllergies.Add(new PatientAllergyModel { PatientId = patient.PatientId, AllergyId = target.AllergyId });
            }
            foreach (var id in medicationIds)
            {
                var source = await _db.Medications.FindAsync(id);
                if (source == null) continue;
                var target = await _db.Set<MedicationModel>().FirstOrDefaultAsync(x => x.MedicationName == source.Name && x.MedicationCategoryId == source.MedicationCategoryId);
                if (target == null) { target = new MedicationModel { MedicationName = source.Name, Description = source.Description ?? "", MedicationCategoryId = source.MedicationCategoryId }; _db.Add(target); await _db.SaveChangesAsync(); }
                _db.PatientMedications.Add(new PatientMedicationModel { PatientId = patient.PatientId, MedicationId = target.MedicationId, StartDate = DateTime.Today });
            }
            await _db.SaveChangesAsync();
            await _email.SendAsync(user.Email!, "Your NMB-HLabSys patient account", $"<p>Hello {System.Net.WebUtility.HtmlEncode(user.FullName)},</p><p>Your patient account has been created.</p><p><b>Username:</b> {System.Net.WebUtility.HtmlEncode(user.Email)}<br/><b>Temporary password:</b> {System.Net.WebUtility.HtmlEncode(temporaryPassword)}</p><p>Sign in and change this temporary password immediately. Do not share it with anyone.</p>");
            TempData["SuccessMessage"] = "Patient created successfully. Login instructions were emailed to the patient.";
            return RedirectToAction(nameof(Details), new { id = patient.PatientId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Patient creation or credential email failed for ID number ending {IdSuffix}", cleanId.Length >= 4 ? cleanId[^4..] : "****");
            // Avoid leaving an account with credentials that were not delivered.
            var patient = await _db.Patients.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (patient != null) { _db.Patients.Remove(patient); await _db.SaveChangesAsync(); }
            await _users.DeleteAsync(user);
            ModelState.AddModelError("", "Patient creation could not be completed because saving the record or sending the credential email failed. Verify database/email configuration and try again.");
            await LoadCatalogsAsync(); return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        var patient = await _db.Patients.Include(p => p.User)
            .Include(p => p.PatientConditions).ThenInclude(x => x.MedicalCondition)
            .Include(p => p.PatientAllergies).ThenInclude(x => x.Allergy)
            .Include(p => p.PatientMedications).ThenInclude(x => x.Medication)
            .FirstOrDefaultAsync(p => p.PatientId == id);
        if (patient == null) return NotFound();
        var userId = patient.UserId;
        var doctorId = _users.GetUserId(User);
        ViewBag.Requests = await _db.TestRequests.Include(r => r.TestTypes).Include(r => r.RequestedTestTypes).ThenInclude(rt => rt.TestType).Include(r => r.Samples)
            .Where(r => r.PatientId == userId && r.DoctorId == doctorId).OrderByDescending(r => r.RequestDate).Take(50).ToListAsync();
        return View(patient);
    }

    [HttpGet]
    public async Task<IActionResult> FindById(string idNumber)
    {
        if (string.IsNullOrWhiteSpace(idNumber)) return Json(new { found = false, message = "Enter the patient's ID number." });
        var patient = await _db.Patients.Include(p => p.User).FirstOrDefaultAsync(p => p.IDNumber == idNumber.Trim());
        if (patient == null) return Json(new { found = false, message = "Patient not found. You can add a new patient.", createUrl = Url.Action(nameof(Create), new { idNumber = idNumber.Trim() }) });
        return Json(new { found = true, patientId = patient.PatientId, name = patient.FullName, dateOfBirth = patient.DateOfBirth.ToString("yyyy-MM-dd"), detailsUrl = Url.Action(nameof(Details), new { id = patient.PatientId }) });
    }

    private async Task LoadCatalogsAsync()
    {
        ViewBag.MedicalConditions = await _db.MedicalConditions.Where(x => x.Status == NMB_HLabSys.Areas.Administrator.Models.Enums.Status.Active).OrderBy(x => x.Name).ToListAsync();
        ViewBag.Allergies = await _db.Allergies.Where(x => x.Status == NMB_HLabSys.Areas.Administrator.Models.Enums.Status.Active).OrderBy(x => x.Name).ToListAsync();
        ViewBag.Medications = await _db.Medications.Where(x => x.Status == NMB_HLabSys.Areas.Administrator.Models.Enums.Status.Active).OrderBy(x => x.Name).ToListAsync();
    }

    private static string GenerateTemporaryPassword()
    {
        const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ"; const string lower = "abcdefghijkmnopqrstuvwxyz";
        const string digits = "23456789"; const string special = "!@$%*-_";
        var chars = new[] { upper[RandomNumberGenerator.GetInt32(upper.Length)], lower[RandomNumberGenerator.GetInt32(lower.Length)], digits[RandomNumberGenerator.GetInt32(digits.Length)], special[RandomNumberGenerator.GetInt32(special.Length)] };
        const string all = upper + lower + digits + special;
        var rest = Enumerable.Range(0, 8).Select(_ => all[RandomNumberGenerator.GetInt32(all.Length)]);
        return new string(chars.Concat(rest).OrderBy(_ => RandomNumberGenerator.GetInt32(int.MaxValue)).ToArray());
    }
}
