using Microsoft.AspNetCore.Mvc;
using global::NMB_HLabSys.Models.Account;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;


namespace NMB_HLabSys.Controllers
    {
        public class AccountController : Controller
        {
        private readonly UserManager<RegisterUser> _userManager;
        private readonly SignInManager<RegisterUser> _signInManager;

        public AccountController(UserManager<RegisterUser> userManager, SignInManager<RegisterUser> signInManager)
            {
                _userManager = userManager;
                _signInManager = signInManager;
            }

            // GET: Register
            [HttpGet]
            public IActionResult RegisterUser()
            {
                return View();
            }


            // GET: Login
            [HttpGet]
            public IActionResult LoginUser()
            {
                return View();
            }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginUser(LoginUser model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Check user roles and redirect accordingly
                if (await _userManager.IsInRoleAsync(user, "Doctor"))
                {
                    return RedirectToAction("Dashboard", "Home");
                }
                else if (await _userManager.IsInRoleAsync(user, "Technician"))
                {
                    return RedirectToAction("Dashboard", "Home", new { area = "Technician" });
                }
                else if (await _userManager.IsInRoleAsync(user, "Manager"))
                {
                    return RedirectToAction("Dashboard", "Home");
                }
                else if (await _userManager.IsInRoleAsync(user, "Admnistrator"))
                {
                    return RedirectToAction("Dashboard", "Home");
                }
                else if (await _userManager.IsInRoleAsync(user, "Patient"))
                {
                    return RedirectToAction("Dashboard", "Home");
                }
                else
                {
                    // Default dashboard for other roles
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Invalid login attempt");
            return View(model);
        }
        // POST: Logout
        [HttpPost]
            public async Task<IActionResult> Logout()
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login");
            }
        }
    }

