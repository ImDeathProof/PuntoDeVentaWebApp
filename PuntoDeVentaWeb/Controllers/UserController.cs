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
using Microsoft.IdentityModel.Tokens;
using PuntoDeVentaWeb.Data;
using PuntoDeVentaWeb.Models;

namespace PuntoDeVentaWeb.Controllers
{
    public class UserController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<UserRole> _roleManager;
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(UserManager<User> userManager, RoleManager<UserRole> roleManager, ILogger<UserController> logger, DataContext context, IUserService userService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _userService = userService;
        }

        [Authorize(Roles = "Owner, Admin, Manager")]
        public async Task<IActionResult> Index()
        {
            var model = new List<UserViewModel>();
            var users = await _userManager.Users.OrderByDescending(u => u.IsActive).ToListAsync();
            foreach (var user in users)
            {
                model.Add(new UserViewModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    LastName = user.LastName,
                    Email = user.Email,
                    UserRole = string.Join(", ", await _userManager.GetRolesAsync(user)),
                    PhoneNumber = user.PhoneNumber,
                    IsActive = user.IsActive
                });
            }
            return View(model);
        }

        [Authorize(Roles = "Owner, Admin, Manager")]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new UserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                LastName = user.LastName,
                Email = user.Email,
                UserRole = string.Join(", ", roles),
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }

        [Authorize(Roles = "Owner, Admin, Manager")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var roles = await LoadRolesAsync(user);
            ViewBag.UserRoles = new SelectList(roles);
            var model = new UserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                LastName = user.LastName,
                Email = user.Email,
                UserRole = string.Join(", ", roles),
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive
            };

            return View(model);
        }
        [Authorize(Roles = "Owner, Admin, Manager")]
        // POST: User/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,LastName,Email,PhoneNumber,IsActive,UserRole")] UserViewModel model)
        {
            if (string.IsNullOrEmpty(id) || model == null)
            {
                return NotFound();
            }

            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userService.GetUserByIdAsync(id);
                    if (user == null)
                    {
                        return NotFound();
                    }
                    user.Name = model.Name;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.NormalizedEmail = model.Email.ToUpper(); // Normalize Email for consistency
                    user.PhoneNumber = model.PhoneNumber;
                    user.UserName = model.Email; // Update UserName to match Email
                    user.NormalizedUserName = model.Email.ToUpper(); // Normalize UserName for consistency
                    if (!await _userService.IsUserInRoleAsync(user.Id, model.UserRole))
                    {
                        if (await _userService.IsUserInRoleAsync(user.Id, "Owner"))
                        {
                            TempData["ErrorMessage"] = "Cannot change the role of the owner user. :)";
                            var roles = await LoadRolesAsync(user);
                            ViewBag.UserRoles = new SelectList(roles);
                            return View(model);
                        }
                        await _userService.RemoveUserFromRoleAsync(user, await _userService.GetUserRoleAsync(user.Id));
                        await _userService.AddUserToRoleAsync(user, model.UserRole);
                    }
                    await _userService.UpdateUserAsync(user);
                    _logger.LogInformation($"User {user.Email} updated successfully.");
                    TempData["SuccessMessage"] = "User updated successfully.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating the user.");
                    TempData["ErrorMessage"] = "An error occurred while updating the user: " + ex.Message;
                    return View(model);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [Authorize(Roles = "Owner, Admin, Manager")]
        // POST: User/Delete/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                if (await _userService.IsUserInRoleAsync(user.Id, "Owner"))
                {
                    TempData["ErrorMessage"] = "Cannot do this action with the owner user. :)";
                    return RedirectToAction(nameof(Index));
                }
                await _userService.DesactivateUserAsync(id);
                _logger.LogInformation($"User {user.Email} disabled successfully.");
                TempData["SuccessMessage"] = "User disabled successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while disabling the user.");
                TempData["ErrorMessage"] = "An error occurred while disabling the user: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "Owner, Admin, Manager")]
        // POST: User/Delete/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enable(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                if (await _userService.IsUserInRoleAsync(user.Id, "Owner"))
                {
                    TempData["ErrorMessage"] = "Cannot do this action with the owner user. :)";
                    return RedirectToAction(nameof(Index));
                }
                await _userService.ActivateUserAsync(id);
                _logger.LogInformation($"User {user.Email} activated successfully.");
                TempData["SuccessMessage"] = "User activated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while activating the user.");
                TempData["ErrorMessage"] = "An error occurred while activating the user: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
        private async Task<List<string>> LoadRolesAsync(User user){
            var roles = new List<string?>
            {
                await _userService.GetUserRoleAsync(user.Id)
            };
            if (await _userService.IsUserInRoleAsync(user.Id, "Owner"))
            {
                var otherRoles = _roleManager.Roles
                    .Where(r => r.Name != _userService.GetUserRoleAsync(user.Id).Result)
                    .Select(r => r.Name)
                    .ToList();
                roles.AddRange(otherRoles);
            }
            else
            {
                var otherRoles = _roleManager.Roles
                    .Where(r => r.Name != _userService.GetUserRoleAsync(user.Id).Result && r.Name != "Owner")
                    .Select(r => r.Name)
                    .ToList();
                roles.AddRange(otherRoles);
            }
            return roles;
        }
    }
}