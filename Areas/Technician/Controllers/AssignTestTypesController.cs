using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NMB_HLabSys.Areas.Technician.Controllers
{
    public class AssignTestTypesController : Controller
    {
        // GET: AssignTestTypesController
        public ActionResult Index()
        {
            return View();
        }

        // GET: AssignTestTypesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AssignTestTypesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AssignTestTypesController/Create
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

        // GET: AssignTestTypesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AssignTestTypesController/Edit/5
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

        // GET: AssignTestTypesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AssignTestTypesController/Delete/5
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
