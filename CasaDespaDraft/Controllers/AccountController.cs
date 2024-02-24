using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CasaDespaDraft.Data;
using CasaDespaDraft.Models;
using CasaDespaDraft.ViewModels;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity.UI.V5.Pages.Account.Manage.Internal;
using System.Net;

namespace CasaDespaDraft.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager, IPasswordHasher<User> passwordHasher)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _passwordHasher = passwordHasher;
        }

        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
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
        public async Task<IActionResult> Register(SignUpViewModel userEnteredData, IFormFile profilePicture)
        {
            if (ModelState.IsValid)
            {
                User newUser = new User();
                newUser.UserName = userEnteredData.email;
                newUser.Firstname = userEnteredData.firstName;
                newUser.Lastname = userEnteredData.lastName;
                newUser.Email = userEnteredData.email;
                newUser.Address = userEnteredData.address;
                newUser.Sex = userEnteredData.sex;
                newUser.Question = userEnteredData.question;
                newUser.Answer = HashPassword(userEnteredData.answer);
                newUser.FAnswer = "";

                if (profilePicture != null && profilePicture.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await profilePicture.CopyToAsync(memoryStream);
                    newUser.ProfilePicture = memoryStream.ToArray();
                }
                else
                {
                    newUser.ProfilePicture = null; // Set recipeImage to null if no image provided
                }


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

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not registered.
                    return RedirectToAction("Register", "Account");
                }

                // For demo purposes, we will get the security question that the user set up during registration.
                // In a real-world scenario, you would send an email to the user with a password reset link.
                var email = user.Email;
                var securityQuestion = user.Question;
                var securityAnswer = user.Answer;
                return RedirectToAction("SecurityQuestion", new { email, securityQuestion, securityAnswer } );
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult SecurityQuestion(string email, string securityQuestion, string securityAnswer)
        {
            var model = new SecurityQuestionViewModel
            {
                Email = email,
                question = securityQuestion,
                answer = securityAnswer 
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult SecurityQuestion(SecurityQuestionViewModel model)
        {
            var fanswer = model.fanswer;
            
            if (ModelState.IsValid)
            {
                // Corrected the variable name for email
                var user = _userManager.Users.FirstOrDefault(u => u.Email == model.Email && fanswer == model.answer);

                if (user != null)
                {
                    // Create a new ResetPasswordViewModel instance and populate the Email property
                    var resetPasswordViewModel = new ResetPasswordViewModel
                    {
                        Email = model.Email, // Set the Email property
                    };

                    // Pass the ResetPasswordViewModel to the ResetPassword view
                    return RedirectToAction("ResetPassword", "Account", new { email = model.Email });
                }
            }

            return RedirectToAction("Gallery", "Home");
        }


        public IActionResult AdminCode()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not registered.
                    return RedirectToAction("ForgotPassword", "Account");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var resetPasswordResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                if (resetPasswordResult.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in resetPasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
    }
}