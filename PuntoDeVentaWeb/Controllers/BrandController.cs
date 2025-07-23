using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PuntoDeVentaWeb.Data;
using PuntoDeVentaWeb.Models;

namespace PuntoDeVentaWeb.Controllers
{
    [Authorize(Roles = "Admin,Owner,Manager")]
    
    public class BrandController : Controller
    {

        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;

        }

        // GET: Brand
        public async Task<IActionResult> Index()
        {
            return View(await _brandService.GetAllBrandsAsync());
        }

        // GET: Brand/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                var brand = await _brandService.GetBrandByIdAsync(id.Value);
                return View(brand);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching brand details: {ex.Message}");
                TempData["ErrorMessage"] = "An error ocurred: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }

        }

        // GET: Brand/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Brand/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Brand brand)
        {
            if (brand == null)
            {
                TempData["ErrorMessage"] = "Brand cannot be null.";
                return View(brand);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // bool exist = await _context.Brands.AnyAsync(b => b.Name.ToLower() == brand.Name.ToLower());
                    // if (exist)
                    // {
                    //     TempData["ErrorMessage"] = "A brand with this name already exists.";
                    //     return View(brand);
                    // }

                    // _context.Add(brand);
                    // await _context.SaveChangesAsync();
                    // TempData["SuccessMessage"] = "Brand created successfully.";
                    await _brandService.AddBrandAsync(brand);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                    TempData["ErrorMessage"] = "An error ocurred: " + ex.Message;
                    return View(brand);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Brand/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _brandService.GetBrandByIdAsync(id.Value);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        // POST: Brand/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Brand brand)
        {
            if (id != brand.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _brandService.UpdateBrandAsync(brand);
                    TempData["SuccessMessage"] = "Brand updated successfully.";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating brand: {ex.Message}");
                    TempData["ErrorMessage"] = "An error ocurred: " + ex.Message;
                    return View(brand);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        // GET: Brand/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            return View(await _brandService.GetBrandByIdAsync(id.Value));
        }

        // POST: Brand/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            try
            {
                await _brandService.DeleteBrandAsync(id);
                TempData["SuccessMessage"] = "Brand deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the exception (ex) if necessary
                Console.WriteLine($"Error deleting brand: {ex.Message}");
                // Set an error message to TempData to display in the view
                TempData["ErrorMessage"] = "An error ocurred: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

    }
}
