using System;
using System.Collections.Generic;
using System.Data;
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
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;

        public ProductController(IProductService productService, ICategoryService categoryService, IBrandService brandService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _brandService = brandService;
        }
        // GET: Product
        public async Task<IActionResult> Index(int? filterId)
        {
            try
            {
                ViewData["CategoryId"] = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name");
                if (filterId == null)
                {
                    return View(await _productService.GetProductsAsync());
                }
                var products = await _productService.GetProductsByCategoryAsync(filterId.Value);
                return View(products);

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while fetching products: " + ex.Message;
                return View(await _productService.GetProductsAsync());
            }
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            try
            {
                ViewData["CategoryId"] = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name");
                if (id == null)
                {
                    TempData["ErrorMessage"] = "Product ID is required.";
                    return RedirectToAction(nameof(Index));
                }

                var product = await _productService.GetProductByIdAsync(id.Value);
                if (product == null)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(product);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while fetching product details: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Product/Create
        public async Task<IActionResult> Create()
        {
            ViewData["BrandId"] = new SelectList(await _brandService.GetAllBrandsAsync(), "Id", "Name");
            ViewData["CategoryId"] = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name");
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,CategoryId,BrandId,SKU,Stock,Price,Image")] Product product)
        {
            try
            {
                ViewData["BrandId"] = new SelectList(await _brandService.GetAllBrandsAsync(), "Id", "Name", product.BrandId);
                ViewData["CategoryId"] = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name", product.CategoryId);

                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Invalid product data.";
                    return View(product);
                }
                product.Price = Math.Round(product.Price, 2); // Ensure price is rounded to 2 decimal places
                await _productService.AddProductAsync(product);
                TempData["SuccessMessage"] = "Product created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while creating the product: " + ex.Message;
                return View(product);  
            }
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try {
                if (id == null)
                {
                    TempData["ErrorMessage"] = "Product ID is required.";
                    return RedirectToAction(nameof(Index));
                }

                var product = await _productService.GetProductByIdAsync(id.Value);
                if (product == null)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    return RedirectToAction(nameof(Index));
                }
                ViewData["BrandId"] = new SelectList(await _brandService.GetAllBrandsAsync(), "Id", "Name", product.BrandId);
                ViewData["CategoryId"] = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name", product.CategoryId);
                return View(product);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while fetching product for edit: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,CategoryId,BrandId,SKU,Stock,Price,Image")] Product product)
        {
            try
            {
                if (id != product.Id)
                {
                    TempData["ErrorMessage"] = "Product ID mismatch.";
                    return View(product);
                }

                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Invalid product data.";
                    ViewData["BrandId"] = new SelectList(await _brandService.GetAllBrandsAsync(), "Id", "Name", product.BrandId);
                    ViewData["CategoryId"] = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name", product.CategoryId);
                    return View(product);
                }

                product.Price = Math.Round(product.Price, 2); // Ensure price is rounded to 2 decimal places
                await _productService.UpdateProductAsync(product);
                TempData["SuccessMessage"] = "Product updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the product: " + ex.Message;
                ViewData["BrandId"] = new SelectList(await _brandService.GetAllBrandsAsync(), "Id", "Name", product.BrandId);
                ViewData["CategoryId"] = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name", product.CategoryId);
                return View(product);
            }
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try {
                if (id == null)
                {
                    TempData["ErrorMessage"] = "Product ID is required.";
                    return RedirectToAction(nameof(Index));
                }

                var product = await _productService.GetProductByIdAsync(id.Value);
                if (product == null)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(product);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while fetching product for deletion: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["ErrorMessage"] = "Invalid product ID.";
                    return RedirectToAction(nameof(Index));
                }
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    return RedirectToAction(nameof(Index));
                }
                await _productService.DeleteProductAsync(product);
                TempData["SuccessMessage"] = "Product deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the product: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
