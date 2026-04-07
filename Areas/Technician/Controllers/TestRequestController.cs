using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Areas.Technician.Models;
using NMB_HLabSys.Data;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    public class TestRequestController : Controller
    {
        // 1. Declare the field
        private readonly ApplicationDbContext _dbcontext;
        public TestRequestController(ApplicationDbContext context)
        {
            _dbcontext = context;
        }

        // GET: TestRequestController
        public ActionResult CreateRequest()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateRequest(TestRequestModel model)
        {
            if (ModelState.IsValid)
            {
                // Example logic: REQ + Year + Month + Random 4 digits
                model.RequestNumber = $"REQ-{DateTime.Now:yyyyMM}-{new Random().Next(1000, 9999)}";

                _dbcontext.Add(model);
                await _dbcontext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // GET: TestRequestController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TestRequestController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TestRequestController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TestRequestController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TestRequestController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TestRequestController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TestRequestController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
