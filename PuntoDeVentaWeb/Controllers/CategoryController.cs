using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PuntoDeVentaWeb.Data;
using PuntoDeVentaWeb.Models;

namespace PuntoDeVentaWeb.Controllers
{
    [Authorize(Roles = "Admin,Owner,Manager")]
    public class CategoryController : Controller
    {
        private readonly DataContext _context;
        private readonly ICategoryService _categoryService;

        public CategoryController(DataContext context, ICategoryService categoryService)
        {
            _categoryService = categoryService;
            _context = context;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            return View(await _categoryService.GetAllCategoriesAsync());
        }

        // GET: Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    TempData["ErrorMessage"] = "Category ID is required.";
                    return RedirectToAction(nameof(Index));
                }

                var category = await _categoryService.GetCategoryByIdAsync(id.Value);
                if (category == null)
                {
                    TempData["ErrorMessage"] = "Category not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(category);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] Category category)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Invalid category data.";
                    return View(category);
                }
                await _categoryService.AddCategoryAsync(category);
                TempData["SuccessMessage"] = "Category created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating category: {ex.Message}");
                TempData["ErrorMessage"] = ex.Message;
                return View(category);
            }
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    TempData["ErrorMessage"] = "Category ID is required.";
                    return RedirectToAction(nameof(Index));
                }

                var category = await _categoryService.GetCategoryByIdAsync(id.Value);
                if (category == null)
                {
                    TempData["ErrorMessage"] = "Category not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(category);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching category details: {ex.Message}");
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Category category)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Invalid category data.";
                    return View(category);
                }
                if (id != category.Id)
                {
                    TempData["ErrorMessage"] = "Category ID mismatch.";
                    return RedirectToAction(nameof(Index));
                }
                await _categoryService.UpdateCategoryAsync(category);
                TempData["SuccessMessage"] = "Category updated successfully.";
                return View(category);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating category: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
                return View(category);
            }
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    TempData["ErrorMessage"] = "Category ID is required.";
                    return RedirectToAction(nameof(Index));
                }

                var category = await _categoryService.GetCategoryByIdAsync(id.Value);
                if (category == null)
                {
                    TempData["ErrorMessage"] = "Category not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(category);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching category for deletion: {ex.Message}");
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            try {
                if(id == null)
                {
                    TempData["ErrorMessage"] = "Category ID is required.";
                    return RedirectToAction(nameof(Index));
                }
                await _categoryService.DeleteCategoryAsync(id.Value);
                TempData["SuccessMessage"] = "Category deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting category: {ex.Message}");
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
