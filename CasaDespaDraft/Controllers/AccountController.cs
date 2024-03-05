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
            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = md5.ComputeHash(bytes);
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
                using (var sha256 = SHA256.Create())
                {
                    byte[] hashedAnswer = sha256.ComputeHash(Encoding.UTF8.GetBytes(userEnteredData.answer.ToUpperInvariant()));
                    newUser.Answer = Convert.ToBase64String(hashedAnswer);
                }
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
                    ModelState.AddModelError("answer", "Account not found.");
                }

                if (model.Email == "admin@example.com")
                {
                    var email = "admin@example.com";
                    return RedirectToAction("AdminCode", new { email });

                }
                else if (user != null)
                {
                    // For demo purposes, we will get the security question that the user set up during registration.
                    // In a real-world scenario, you would send an email to the user with a password reset link.
                    var email = user.Email;
                    var securityQuestion = user.Question;
                    var securityAnswer = user.Answer;


                    return RedirectToAction("SecurityQuestion", new { email, securityQuestion, securityAnswer });
                }

            }

            // Add a ModelState error if the answer is incorrect
            ModelState.AddModelError("answer", "Account not found.");


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
        public async Task<IActionResult> SecurityQuestion(SecurityQuestionViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    user.Email = model.Email;
                    user.Answer = model.answer;
                    using (var sha256 = SHA256.Create())
                    {
                        byte[] hashedFAnswer = sha256.ComputeHash(Encoding.UTF8.GetBytes(model.fanswer.ToUpperInvariant()));
                        user.FAnswer = Convert.ToBase64String(hashedFAnswer);
                    }

                    if (user.FAnswer == user.Answer)
                    {
                        // Create a new ResetPasswordViewModel instance and populate the Email property
                        var resetPasswordViewModel = new ResetPasswordViewModel
                        {
                            Email = model.Email, // Set the Email property
                        };

                        var email = user.Email;
                        var firstname = user.Firstname;
                        var lastname = user.Lastname;
                        var address = user.Address;
                        var sex = user.Sex;
                        var fanswer = user.FAnswer;
                        var question = user.Question;
                        var answer = user.Answer;
                        var passwordHash = user.PasswordHash;
                        return RedirectToAction("ResetPassword", "Account", new { email, firstname, lastname, address, sex, fanswer, question, answer, passwordHash });
                    }
                    else
                    {
                        // Add a ModelState error if the answer is incorrect
                        ModelState.AddModelError("answer", "The answer you entered is incorrect.");
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string firstname, string lastname, string address, string sex, string fanswer, string question, string answer, string passwordHash)
        {

            var model = new SignUpViewModel
            {
                email = email,
                firstName = firstname,
                lastName = lastname,
                address = address,
                sex = sex,
                fanswer = fanswer,
                question = question,
                answer = answer,
                userPassword = passwordHash

            };


            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(SignUpViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.email);
            user.UserName = model.email;
            user.Firstname = model.firstName;
            user.Lastname = model.lastName;
            user.Email = model.email;
            user.Address = model.address;
            user.Sex = model.sex;
            user.Question = model.question;
            user.Answer = model.answer;
            user.FAnswer = "";


            if (ModelState.IsValid)
            {
                //Update Password
                if (!string.IsNullOrEmpty(model.userPassword))
                {
                    var passwordValidator = _userManager.PasswordValidators.FirstOrDefault();
                    var passwordValidationResult = await passwordValidator.ValidateAsync(_userManager, user, model.userPassword);

                    if (!passwordValidationResult.Succeeded)
                    {
                        foreach (var error in passwordValidationResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        ModelState.AddModelError("answer", "The Password should have a minimum of 8 characters, at least 1 Upper Case Letter, 1 Lower Case, 1 Special Character");
                    }
                    else
                    {

                        // If validation succeeded, update the password hash
                        var newPasswordHash = _userManager.PasswordHasher.HashPassword(user, model.userPassword);
                        user.PasswordHash = newPasswordHash;

                        var result = await _userManager.UpdateAsync(user);

                        if (result.Succeeded)
                        {
                            TempData["SuccessMessage"] = "Account Updated Successfully!";
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                }
            }

            ModelState.AddModelError("answer", "Passwords do not match");

            return View(model);

        }

        [HttpGet]
        public IActionResult AdminCode(string email)
        {
            var model = new AdminCodeViewModel
            {
                Email = email
            };


            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminCode(AdminCodeViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    user.Email = model.Email;

                    if (model.adminCode.Equals("Despabiladeras", StringComparison.OrdinalIgnoreCase))
                    {
                        var userId = user.Id;
                        var email = user.Email;
                        var firstname = user.Firstname;
                        var lastname = user.Lastname;
                        var passwordHash = user.PasswordHash;
                        var address = user.Address;
                        var sex = user.Sex;
                        var fanswer = user.FAnswer;
                        var question = user.Question;
                        var answer = user.Answer;
                        return RedirectToAction("AdminResetPassword", "Account", new { userId, email, firstname, lastname, address, sex, fanswer, question, answer, passwordHash });
                    }
                    else
                    {
                        // Add a ModelState error if the answer is incorrect
                        ModelState.AddModelError("answer", "The admin code you entered is incorrect.");
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult AdminResetPassword(int userId, string email, string firstname, string lastname, string address, string sex, string fanswer, string question, string answer, string passwordHash)
        {

            var model = new AdminResetPasswordViewModel
            {
                userId = userId,
                email = email,
                firstName = firstname,
                lastName = lastname,
                userPassword = passwordHash

            };


            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminResetPassword(AdminResetPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.email);
            user.UserName = model.email;
            user.Firstname = model.firstName;
            user.Lastname = model.lastName;
            user.Email = model.email;

            if (ModelState.IsValid)
            {

                //Update Password
                if (!string.IsNullOrEmpty(model.userPassword))
                {
                    var passwordValidator = _userManager.PasswordValidators.FirstOrDefault();
                    var passwordValidationResult = await passwordValidator.ValidateAsync(_userManager, user, model.userPassword);

                    if (!passwordValidationResult.Succeeded)
                    {
                        foreach (var error in passwordValidationResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        ModelState.AddModelError("answer", "The Password should have a minimum of 8 characters, at least 1 Upper Case Letter, 1 Lower Case, 1 Special Character");
                    }
                    else
                    {

                        // If validation succeeded, update the password hash
                        var newPasswordHash = _userManager.PasswordHasher.HashPassword(user, model.userPassword);
                        user.PasswordHash = newPasswordHash;

                        var result = await _userManager.UpdateAsync(user);

                        if (result.Succeeded)
                        {
                            TempData["SuccessMessage"] = "Account Updated Successfully!";
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                }
            }

            ModelState.AddModelError("answer", "Passwords do not match");

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                var profileData = new SignUpViewModel
                {
                    firstName = user.Firstname,
                    lastName = user.Lastname,
                    address = user.Address,
                    sex = user.Sex,
                    question = user.Question,
                    answer = user.Answer,
                    profilePicture = user.ProfilePicture,
                    fanswer = user.FAnswer,
                    email = user.Email,
                    userPassword =  user.PasswordHash,
                    ConfirmPassword = user.PasswordHash
                };

                return View(profileData);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(SignUpViewModel updatedProfile, IFormFile? profilePicture)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                //Update User Profile
                if (user != null)
                {
                    user.Firstname = updatedProfile.firstName;
                    user.Lastname = updatedProfile.lastName;
                    user.Address = updatedProfile.address;
                    user.Sex = updatedProfile.sex;
                    user.ProfilePicture = updatedProfile.profilePicture;
                    user.Answer = updatedProfile.answer;
                    user.Question = updatedProfile.question;
                    user.FAnswer = updatedProfile.fanswer;
                    user.Email = updatedProfile.email;

                    if (profilePicture != null && profilePicture.Length > 0)
                    {
                        using var memoryStream = new MemoryStream();
                        await profilePicture.CopyToAsync(memoryStream);
                        user.ProfilePicture = memoryStream.ToArray();
                    }
                    else
                    {
                        user.ProfilePicture = null;
                    }


                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        TempData["SuccessMessage"] = "Account Updated Successfully!";

                        return RedirectToAction("Profile", "Home");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                else
                {
                    return NotFound();
                }
            }

            // If ModelState is not valid, return to the view with the current data
            return View(updatedProfile);
        }

    }
}