using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CodeVaultMVC.Models;
using CodeVaultMVC.ViewModels;
using System.Text.Json;

namespace CodeVaultMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Users> _signInManager;
        private readonly UserManager<Users> _userManager;

        public AccountController(SignInManager<Users> signInManager, UserManager<Users> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult ChooseRole()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                    return RedirectToAction("Index", "Admin");
                else
                    return RedirectToAction("Index", "User");
            }
            return View();
        }

        public IActionResult Login(string role)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                    return RedirectToAction("Index", "Admin");
                else
                    return RedirectToAction("Index", "User");
            }
            if (string.IsNullOrEmpty(role))
                return RedirectToAction("ChooseRole");
            ViewBag.SelectedRole = role;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string role)
        {
            if (string.IsNullOrEmpty(role))
                return RedirectToAction("ChooseRole");
            ViewBag.SelectedRole = role;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

                        if (role == "Admin" && isAdmin)
                            return RedirectToAction("Index", "Admin");
                        else if (role == "Developer" && !isAdmin)
                            return RedirectToAction("Index", "User");
                        else
                        {
                            await _signInManager.SignOutAsync();
                            ModelState.AddModelError("", "Seçtiğiniz rol ile kullanıcı hesabınız uyuşmuyor!");
                            return View(model);
                        }
                    }
                }

                ModelState.AddModelError("", "Hatalı giriş denemesi!");
            }
            return View(model);
        }

        public IActionResult Register(string role)
        {
            if (string.IsNullOrEmpty(role))
                return RedirectToAction("ChooseRole");
            ViewBag.SelectedRole = role;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model, string role)
        {
            if (string.IsNullOrEmpty(role))
                return RedirectToAction("ChooseRole");
            ViewBag.SelectedRole = role;

            if (ModelState.IsValid)
            {
                Users user = new Users { FullName = model.Name, Email = model.Email, UserName = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (role == "Admin")
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
                    }
                    else if (role == "Developer")
                    {
                        try
                        {
                            using (var client = new HttpClient())
                            {
                                var devModel = new
                                {
                                    FullName = model.Name,
                                    EMail = model.Email,
                                    GithubUrl = "",
                                    LinkedinUrl = ""
                                };
                                var content = new StringContent(JsonSerializer.Serialize(devModel), System.Text.Encoding.UTF8);
                                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                                await client.PostAsync("https://localhost:7000/api/Developers/AddDevelopers", content);
                            }
                        }
                        catch { }
                    }
                    return RedirectToAction("Login", "Account", new { role = role });
                }
                foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        public IActionResult VerifyEmail(string role)
        {
            if (string.IsNullOrEmpty(role))
                return RedirectToAction("ChooseRole");
            ViewBag.SelectedRole = role;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model, string role)
        {
            if (string.IsNullOrEmpty(role))
                return RedirectToAction("ChooseRole");
            ViewBag.SelectedRole = role;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user != null) return RedirectToAction("ChangePassword", "Account", new { username = user.UserName, role = role });
                ModelState.AddModelError("", "Bu e-posta kayıtlı değil!");
            }
            return View(model);
        }

        public IActionResult ChangePassword(string username, string role)
        {
            if (string.IsNullOrEmpty(role))
                return RedirectToAction("ChooseRole");
            ViewBag.SelectedRole = role;
            if (string.IsNullOrEmpty(username)) return RedirectToAction("VerifyEmail", new { role = role });
            return View(new ChangePasswordViewModel { Email = username });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model, string role)
        {
            if (string.IsNullOrEmpty(role))
                return RedirectToAction("ChooseRole");
            ViewBag.SelectedRole = role;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    await _userManager.RemovePasswordAsync(user);
                    var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                    if (result.Succeeded) return RedirectToAction("Login", "Account", new { role = role });
                    foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
                }
                else ModelState.AddModelError("", "Kullanıcı bulunamadı!");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("ChooseRole");
        }
    }
}