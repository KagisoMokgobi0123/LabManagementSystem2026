using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NMB_HLabSys.Areas.Manager.Controllers
{
    public class TestTypeController : Controller
    {
        // GET: TestTypeController
        public ActionResult Index()
        {
            return View();
        }

        // GET: TestTypeController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TestTypeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TestTypeController/Create
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

        // GET: TestTypeController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TestTypeController/Edit/5
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

        // GET: TestTypeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TestTypeController/Delete/5
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
