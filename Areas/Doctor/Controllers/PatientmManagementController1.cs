using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NMB_HLabSys.Areas.Doctor.Controllers;

[Area("Doctor"), Authorize(Roles = "Doctor")]
public sealed class PatientmManagementController1 : Controller
{
    [HttpGet]
    public IActionResult Index() => RedirectToAction("Index", "Patients", new { area = "Doctor" });
}
