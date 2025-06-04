using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PuntoDeVentaWeb.Data;
using PuntoDeVentaWeb.Models;

namespace PuntoDeVentaWeb.Controllers
{
    public class AccountController : Controller
        {
            private readonly UserManager<User> _userManager;
            private readonly RoleManager<UserRole> _roleManager;
            private readonly SignInManager<User> _signInManager;
            private readonly ILogger<AccountController> _logger;


            public AccountController(
                UserManager<User> userManager,
                SignInManager<User> signInManager,
                ILogger<AccountController> logger, 
                RoleManager<UserRole> roleManager)
            {
                _userManager = userManager;
                _signInManager = signInManager;
                _logger = logger;
                _roleManager = roleManager;
            }

            [HttpGet]
            [AllowAnonymous]
            public IActionResult Login(string? returnUrl = null)
            {
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }

            [HttpPost]
            [AllowAnonymous]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
            {
                ViewData["ReturnUrl"] = returnUrl;
                
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                    if (!user.IsActive)
                        {
                            ModelState.AddModelError(string.Empty, "User is disabled. Please contact support.");
                            return View(model);
                        }    
                    }
                    var result = await _signInManager.PasswordSignInAsync(
                        model.Email, 
                        model.Password, 
                        model.RememberMe, 
                        lockoutOnFailure: false);
                        
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User authenticated.");
                        return RedirectToAction("Profile", "Account");
                        
                    }
                    ModelState.AddModelError(string.Empty, "User or password is wrong.");
                }
                return View(model);
            }

            [HttpGet]
            [Authorize(Roles = "Owner,Admin")]
            public IActionResult Register()
            {
                var roles = _roleManager.Roles
                    .Where(r => r.Name != "Owner")
                    .Select(r => r.Name)
                    .ToList();
                ViewBag.UserRoles = new SelectList(roles);
                return View();
            }

            [HttpPost]
            [Authorize(Roles = "Owner,Admin")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Register(RegisterViewModel model)
            {
                if (ModelState.IsValid)
                {
                    Console.WriteLine("Model is valid, proceeding with registration.");
                    Console.WriteLine($"Email: {model.Email}, Password: {model.Password}, Role: {model.SelectedRole}");
                    var user = new User
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        Name = model.Name,
                        LastName = model.LastName,
                        PhoneNumber = model.PhoneNumber,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    //Saving the user
                    Console.WriteLine("Creating user in UserManager.");
                    var result = await _userManager.CreateAsync(user, model.Password);
                    // Check if the user was created successfully
                    if (!result.Succeeded)
                    {
                        TempData["ErrorMessage"] = "An error occurred while creating the user. Please try again.";
                        return RedirectToAction("Register");
                    }
                    //Delay a little to ensure the user is created
                    await Task.Delay(100);
                    // Check if the user was created successfully
                    var createdUser = await _userManager.FindByEmailAsync(model.Email);
                    if (createdUser == null)
                    {
                        TempData["ErrorMessage"] = "An error occurred while creating the user. Please try again.";
                    }
                    // Add the user to the selected role
                    var roleResult = await _userManager.AddToRoleAsync(user, model.SelectedRole);
                    if (!roleResult.Succeeded)
                    {
                        TempData["ErrorMessage"] = "An error occurred while assigning the role. Please try again.";
                        return RedirectToAction("Register");
                    }

                    _logger.LogInformation($"New user registered with rol {model.SelectedRole}: {model.Email}");
                    return RedirectToAction("Index", "User");
                    
                }
                var roles = _roleManager.Roles
                    .Where(r => r.Name != "Owner")
                    .Select(r => r.Name)
                    .ToList();
                ViewBag.UserRoles = new SelectList(roles);
                return View(model);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Logout()
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation("Usuario cerró sesión.");
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            private void AddErrors(IdentityResult result)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            private IActionResult RedirectToLocal(string? returnUrl)
            {
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            [Authorize]
            
            public async Task<IActionResult> Profile()
            {
                ViewData["Title"] = "User Profile";

                var user = await _userManager.GetUserAsync(User);
                
                if (user == null)
                {
                    // Redirect to login if user is not found
                    Console.WriteLine("User not found, redirecting to login.");
                    return RedirectToAction("Login", "Account");
                }
                Console.WriteLine("User found, building profile view model.");

                var model = new ProfileViewModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                };
                return View(model);
            }

            [HttpPost]
            [Authorize]
            public async Task<IActionResult> Profile(ProfileViewModel model)
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(User);
                    user.Id = model.Id;
                    user.PhoneNumber = model.PhoneNumber;
                    user.Name = model.Name;
                    user.LastName = model.LastName;
                    user.Email = model.Email;

                    
                    await _userManager.UpdateAsync(user);
                    return RedirectToAction(nameof(Profile));
                }
                return View(model);
            }

            [HttpPost]
            [Authorize]
            public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(User);
                    var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    
                    if (result.Succeeded)
                    {
                        await _signInManager.RefreshSignInAsync(user);
                        return RedirectToAction(nameof(Profile), new { message = "Contraseña actualizada" });
                    }
                    
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                return View("Profile", await BuildProfileViewModelAsync());
            }

            private async Task<ProfileViewModel> BuildProfileViewModelAsync()
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new InvalidOperationException("User not found.");
                }

                return new ProfileViewModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                };
            }

            [AllowAnonymous]
            public IActionResult ForgotPassword()
            {
                return View();
            }

            [HttpPost]
            [AllowAnonymous]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var callbackUrl = Url.Action("ResetPassword", "Account", 
                            new { token, email = user.Email }, protocol: HttpContext.Request.Scheme);
                        
                        // Implementa aquí el envío de email (usando SendGrid, MailKit, etc.)
                        // Ejemplo simplificado:
                        // await _emailService.SendResetPasswordEmail(user.Email, callbackUrl);
                        
                        return View("ForgotPasswordConfirmation");
                    }
                    // No revelar que el usuario no existe
                    return View("ForgotPasswordConfirmation");
                }
                return View(model);
            }

            [AllowAnonymous]
            public IActionResult ResetPassword(string token, string email)
            {
                var model = new ResetPasswordViewModel { Token = token, Email = email };
                return View(model);
            }

            [HttpPost]
            [AllowAnonymous]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
            {
                if (!ModelState.IsValid)
                    return View(model);
                
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    return RedirectToAction(nameof(ResetPasswordConfirmation));
                
                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                    return RedirectToAction(nameof(ResetPasswordConfirmation));
                
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                
                return View();
            }

            private object ResetPasswordConfirmation()
            {
                throw new NotImplementedException();
            }
    }
}