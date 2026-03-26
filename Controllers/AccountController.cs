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

            // POST: Register
            [HttpPost]
            public async Task<IActionResult> RegisterIUser(RegisterUser model)
            {
            if (!ModelState.IsValid)
                return View("RegisterUser", model);

               var user = new RegisterUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Optional: automatically sign in user after registration
                    // await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }

            // GET: Login
            [HttpGet]
            public IActionResult LoginUser()
            {
                return View();
            }

            // POST: Login
            [HttpPost]
            public async Task<IActionResult> LoginUser(LoginUser model)
            {
                if (!ModelState.IsValid)
                    return View(model);

                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home"); // Or your dashboard
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

