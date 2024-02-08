using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CasaDespaDraft.Data;
using CasaDespaDraft.Models;
using CasaDespaDraft.ViewModels;

namespace CasaDespaDraft.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(SignInViewModel loginInfo)
        {
            var result = await _signInManager.PasswordSignInAsync(loginInfo.UserName, loginInfo.Password, loginInfo.RememberMe, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(loginInfo.UserName);

                if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    return RedirectToAction("Index", "Home"); // Redirect admin to the dashboard
                }

                return RedirectToAction("Index", "Home"); // Redirect regular users to the homepage
            }
            else
            {
                ModelState.AddModelError("", "User Credentials Do Not Match");
            }
            return View(loginInfo);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(SignUpViewModel userEnteredData)
        {
            if (ModelState.IsValid)
            {
                User newUser = new User();
                newUser.UserName = userEnteredData.email;
                newUser.Firstname = userEnteredData.firstName;
                newUser.Lastname = userEnteredData.lastName;
                newUser.Email = userEnteredData.email;
                newUser.Address = userEnteredData.address;
                newUser.Question = userEnteredData.question;
                newUser.Answer = userEnteredData.answer;


                var result = await _userManager.CreateAsync(newUser, userEnteredData.userPassword);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Registration successful!, You may now Login";
                    return RedirectToAction("Register", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(userEnteredData);
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult SecurityQuestion()
        {
            return View();
        }

        public IActionResult AdminCode()
        {
            return View();
        }

    }
}