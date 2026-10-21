using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Data;

namespace NMB_HLabSys.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = "Manager")]
    public class SampleTypeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SampleTypeController(ApplicationDbContext context)
        {
            _context = context;
        }




        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var sampleTypes = await GetActiveSampleTypes();

            return View(sampleTypes);
        }



        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var sampleType = await _context.SampleTypes
                .Include(s => s.Samples)
                .FirstOrDefaultAsync(s =>
                    s.SampleTypeId == id);

            if (sampleType == null)
                return NotFound();

            return Json(new
            {
                success = true,
                sampleType = new
                {
                    sampleType.SampleTypeId,
                    sampleType.Name,

                    SampleCount =
                        sampleType.Samples.Count
                }
            });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            SampleTypeModel model)
        {
            model.Name =
                model.Name?.Trim() ?? string.Empty;


            await ValidateSampleType(model);


            // duplicate sample check

            bool duplicateSampleType =
                await _context.SampleTypes
                    .AnyAsync(s =>
                        s.Name.ToLower() ==
                        model.Name.ToLower());

            if (duplicateSampleType)
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "A sample type with this name already exists.");
            }


            if (!ModelState.IsValid)
            {
                var sampleTypes =
                    await GetActiveSampleTypes();

                return View("Index", sampleTypes);
            }



            model.SampleTypeId = 0;

            // New sample types are active by default.
            model.IsActive = true;

            _context.SampleTypes.Add(model);

            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                $"Sample type '{model.Name}' added successfully.";


            return RedirectToAction(
                nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            SampleTypeModel model)
        {
            model.Name =
                model.Name?.Trim() ?? string.Empty;


            // Find Existing sample

            var existing =
                await _context.SampleTypes
                    .FirstOrDefaultAsync(s =>
                        s.SampleTypeId ==
                        model.SampleTypeId);

            if (existing == null)
                return NotFound();


            await ValidateSampleType(model);


            //duplicate Check

            bool duplicateSampleType =
                await _context.SampleTypes
                    .AnyAsync(s =>
                        s.SampleTypeId !=
                        model.SampleTypeId &&
                        s.Name.ToLower() ==
                        model.Name.ToLower());


            if (duplicateSampleType)
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "A sample type with this name already exists.");
            }


            if (!ModelState.IsValid)
            {
                var sampleTypes =
                    await GetActiveSampleTypes();

                return View("Index", sampleTypes);
            }



            existing.Name =
                model.Name;


            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                $"Sample type '{existing.Name}' updated successfully.";


            return RedirectToAction(
                nameof(Index));
        }

//soft delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(
            int id)
        {
            var sampleType =
                await _context.SampleTypes
                    .Include(s => s.Samples)
                    .FirstOrDefaultAsync(s =>
                        s.SampleTypeId == id);


            if (sampleType == null)
            {
                TempData["ErrorMessage"] =
                    "Sample type could not be found.";

                return RedirectToAction(
                    nameof(Index));
            }


            bool hasSamples =
                await _context.Samples
                    .AnyAsync(s =>
                        s.SampleTypeId == id);


            if (hasSamples)
            {
                TempData["ErrorMessage"] =
                    "This sample type cannot be deleted because it is linked to one or more samples.";

                return RedirectToAction(
                    nameof(Index));
            }


            sampleType.IsActive = false;

            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                $"Sample type '{sampleType.Name}' deleted successfully.";


            return RedirectToAction(
                nameof(Index));
        }


     

        private async Task ValidateSampleType(
            SampleTypeModel model)
        {


            if (string.IsNullOrWhiteSpace(
                model.Name))
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "Sample type name is required.");
            }


            // max Length

            if (model.Name.Length > 100)
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "Sample type name cannot exceed 100 characters.");
            }
        }


        private async Task<List<SampleTypeModel>>
            GetActiveSampleTypes()
        {
            return await _context.SampleTypes
                .Include(s => s.Samples)
                .Where(s =>
                    s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }
    }


}