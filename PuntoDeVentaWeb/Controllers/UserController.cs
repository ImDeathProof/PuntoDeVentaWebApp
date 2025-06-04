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
    public class UserController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<UserRole> _roleManager;
        private readonly ILogger<UserController> _logger;

        public UserController(UserManager<User> userManager, RoleManager<UserRole> roleManager, ILogger<UserController> logger, DataContext context)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [Authorize(Roles = "Owner, Admin, Manager")]
        public async Task<IActionResult> Index()
        {
            var model = new List<UserViewModel>();
            var users = await _userManager.Users.ToListAsync();
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
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive
            };

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
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            if (await _userManager.IsInRoleAsync(user, "Owner"))
            {
                TempData["ErrorMessage"] = "Cannot do this action with the owner user. :)";
                return RedirectToAction(nameof(Index));
            }
            user.IsActive = false;
            user.DesactivatedDate = DateTime.UtcNow;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation($"User {user.Email} disabled successfully.");
                TempData["SuccessMessage"] = "User disabled successfully.";
            }
            return RedirectToAction(nameof(Edit), new { id = user.Id });
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
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            if(await _userManager.IsInRoleAsync(user, "Owner"))
            {
                TempData["ErrorMessage"] = "Cannot do this action with the owner user. :)";
                return RedirectToAction(nameof(Index));
            }
            user.IsActive = true;
            user.DesactivatedDate = null;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation($"User {user.Email} enabled successfully.");
                TempData["SuccessMessage"] = "User enabled successfully.";
            }
            return RedirectToAction(nameof(Edit), new { id = user.Id });
        }
    }
}