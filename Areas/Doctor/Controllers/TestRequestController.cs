using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NMB_HLabSys.Areas.Doctor.Controllers
{
    public class TestRequestController : Controller
    {
        // GET: CreateTestRequestController
        public ActionResult Index()
        {
            return View();
        }

        // GET: CreateTestRequestController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CreateTestRequestController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CreateTestRequestController/Create
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

        // GET: CreateTestRequestController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CreateTestRequestController/Edit/5
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

        // GET: CreateTestRequestController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CreateTestRequestController/Delete/5
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
