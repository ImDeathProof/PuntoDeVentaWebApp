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
    [Authorize]
    public class SaleController : Controller
    {
        private readonly DataContext _context;
        private readonly ISaleService _saleService;
        private readonly IProductService _productService;

        public SaleController(DataContext context, ISaleService saleService, IProductService productService)
        {
            _context = context;
            _saleService = saleService;
            _productService = productService;
        }
        // GET: Sale
        public async Task<IActionResult> Index(string search)
        {
            ViewData["CurrentSearch"] = search;
            var sales = _context.Sales.Include(s => s.Client)
                        .Include(s => s.Status)
                        .Include(s => s.User)
                        .Include(s => s.PaymentMethod)
                        .AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                sales = sales.Where(s => s.Client.Name.Contains(search) || s.Client.Email.Contains(search));
            }
            return View(await sales.ToListAsync());
        }
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name");
            ViewData["SellerId"] = new SelectList(_context.Users, "Id", "Name");
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Name");
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "Id", "Name");
            var products = _context.Products
                .Where(p => p.Stock > 0)
                .Select(p => new { p.Id, p.Name })
                .ToList();
            ViewData["ProductId"] = new SelectList(products, "Id", "Name");
            var model = new SalesViewModel
            {
                Sale = new Sale(),
                SaleDetails = new List<SaleDetail> { new SaleDetail() }
            };
            model.Sale.Date = DateTime.Now;
            return View(model);
        }
        // POST: Sale/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SalesViewModel model)
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name");
            ViewData["SellerId"] = new SelectList(_context.Users, "Id", "Name");
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Name");
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "Id", "Name");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            //if (ModelState.IsValid)
            //{
            // Validate the sale details
            if (!model.SaleDetails.Any())
            {
                ModelState.AddModelError("SaleDetails", "At least one sale detail is required.");
                return View(model);
            }
            // Check the stock for each product in the sale details
            foreach (var detail in model.SaleDetails)
            {
                if (detail.Quantity <= 0)
                {
                    ModelState.AddModelError("SaleDetails", "Invalid quantity in sale details.");
                    return View(model);
                }
                var product = await _context.Products.FindAsync(detail.ProductId);
                if (product == null || product.Stock < detail.Quantity)
                {
                    ModelState.AddModelError("SaleDetails", $"Insufficient stock for product {product?.Name ?? "Unknown"}.");
                    return View(model);
                }
            }
            try
            {
                model.Sale.StatusId = 1; // Assuming 1 is the ID for "Pending" status
                _context.Add(model.Sale);
                await _context.SaveChangesAsync();

                if (model.SaleDetails.Count() > 0)
                {
                    foreach (var detail in model.SaleDetails)
                    {
                        if (detail.ProductId == 0 || detail.Quantity <= 0)
                        {
                            continue; // Skip invalid details
                        }
                        detail.SaleId = model.Sale.Id;
                        _context.Add(detail);
                        // Update product stock
                        await _productService.removeStockAsync(detail.ProductId, detail.Quantity);
                    }
                }
                await _context.SaveChangesAsync();
                // Update Sale total
                await _saleService.UpdateSaleTotalAsync(model.Sale.Id);
                TempData["SuccessMessage"] = "Sale created successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while creating the sale";
                Console.WriteLine(ex.Message);
                return View(model);
            }
            //}

        }

        public async Task<IActionResult> Details(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.Client)
                .Include(s => s.User)
                .Include(s => s.Status)
                .Include(s => s.PaymentMethod)
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }

            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", sale.ClientId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", sale.UserId);
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Name", sale.StatusId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "Id", "Name", sale.PaymentMethodId);

            return View(sale);
        }
        // POST: Sale/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClientId,Total,Date,UserId,StatusId,PaymentMethodId")] Sale sale)
        {
            if (id != sale.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sale);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Sale updated successfully";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while updating the sale";
                    Console.WriteLine(ex.Message);
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", sale.ClientId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", sale.UserId);
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Name", sale.StatusId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "Id", "Name", sale.PaymentMethodId);
            ViewData["ErrorMessage"] = "Something went wrong while updating the sale. Please try again.";
            return View(sale);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.Client)
                .Include(s => s.User)
                .Include(s => s.Status)
                .Include(s => s.PaymentMethod)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }
        // POST: Sale/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale != null)
            {
                try
                {
                    // delete sale
                    await _saleService.DeleteSaleAsync(sale);
                    // delete vinculed details
                    await _saleService.DeleteVinculedDetailsAsync(id);
                    TempData["SuccessMessage"] = "Sale deleted successfully";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while deleting the sale";
                    Console.WriteLine(ex.Message);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SaleExists(int id)
        {
            return _context.Sales.Any(e => e.Id == id);
        }
    }
}