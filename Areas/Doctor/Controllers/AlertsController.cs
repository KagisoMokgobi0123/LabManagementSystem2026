using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Doctor.Models;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;

namespace NMB_HLabSys.Areas.Doctor.Controllers;

[Area("Doctor"), Authorize(Roles = "Doctor")]
public sealed class AlertsController : Controller
{
    private readonly ApplicationDbContext _db; private readonly UserManager<ApplicationUser> _users;
    public AlertsController(ApplicationDbContext db, UserManager<ApplicationUser> users) { _db = db; _users = users; }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var doctorId = _users.GetUserId(User);
        var requests = await _db.TestRequests.Include(r => r.Patient).Include(r => r.Samples)
            .Include(r => r.TestTypes).Include(r => r.RequestedTestTypes).ThenInclude(rt => rt.TestType)
            .Where(r => r.DoctorId == doctorId && (r.Status == "Completed" || r.Status == "Released" || r.Status == "ReleasedByDoctor"))
            .ToListAsync();
        var ids = requests.Where(r => r.Samples.Any() && r.Samples.All(s => s.IsReceived) &&
            r.EffectiveTestTypes.Count > 0 && r.RequestedTestTypes.Count(x => x.Status == "Completed") >= r.EffectiveTestTypes.Count &&
            r.RequestedTestTypes.All(x => x.Status == "Completed"))
            .Select(r => r.TestRequestId).ToList();
        var eligible = requests.Where(r => ids.Contains(r.TestRequestId)).ToDictionary(x => x.TestRequestId);
        var abnormal = await _db.TestRequestTestTypes.Include(x => x.TestType).ThenInclude(t => t!.MeasurementUnitDetails)
            .Where(x => ids.Contains(x.TestRequestId) && x.IsAbnormal && x.Status == "Completed")
            .OrderByDescending(x => x.CompletionDateTime).ToListAsync();
        ViewBag.Requests = eligible;
        return View(abnormal);
    }
}
