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
    public class PurchaseController : Controller
    {
        private readonly DataContext _context;
        private readonly IPurchaseService _purchaseService;
        private readonly IProductService _productService;

        public PurchaseController(DataContext context, IPurchaseService purchaseService, IProductService productService)
        {
            _purchaseService = purchaseService;
            _context = context;
            _productService = productService;
        }

        // GET: Purchase
        public async Task<IActionResult> Index(string search)
        {
            ViewData["Search"] = search;
            var purchases = _context.Purchases
                .Include(p => p.PaymentMethod)
                .Include(p => p.Supplier)
                .Include(p => p.User)
                .AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                purchases = purchases.Where(p => p.Supplier.Name.Contains(search) ||
                                                  p.User.Name.Contains(search) ||
                                                  p.User.LastName.Contains(search));
            }
            if (!string.IsNullOrEmpty(search))
            {
                if (purchases.Count() == 0)
                {
                    TempData["ErrorMessage"] = "No purchases found. Please add a purchase.";
                }
                else
                {
                    TempData["SuccessMessage"] = $"{purchases.Count()} purchases found.";
                }
            }
            return View(await purchases.ToListAsync());
        }

        // GET: Purchase/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases
                .Include(p => p.PaymentMethod)
                .Include(p => p.Supplier)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }
        public IActionResult Create()
        {
            var model = new PurchaseViewModel
            {
                Purchase = new Purchase(),
                PurchaseDetails = new List<PurchaseDetail> { new PurchaseDetail() } // 1 fila vacía inicial
            };
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.Users.Where(u => u.IsActive == true), "Id", "Name");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            ViewData["PaymentMethodsId"] = new SelectList(_context.PaymentMethods, "Id", "Name");
            return View(model);
        }
        // // POST: Purchase/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseViewModel model)
        {
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.Users.Where(u => u.IsActive == true), "Id", "Name");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            ViewData["PaymentMethodsId"] = new SelectList(_context.PaymentMethods, "Id", "Name");

            if (!ModelState.IsValid)
            {
                // Loggear errores de validación
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            try
            {
                // Asigns date if not set
                if (model.Purchase.Date == default)
                {
                    model.Purchase.Date = DateTime.Now;
                }

                // Check empty
                model.PurchaseDetails = model.PurchaseDetails?
                    .Where(d => d.ProductId > 0 && d.Quantity > 0)
                    .ToList();

                if (model.PurchaseDetails == null || !model.PurchaseDetails.Any())
                {
                    TempData["ErrorMessage"] = "Most add one detail at least.";
                    return View(model);
                }
                if (model.Purchase.SupplierId <= 0)
                {
                    TempData["ErrorMessage"] = "You must select a valid supplier.";
                    return View(model);
                }
                if (model.Purchase.PaymentMethodId <= 0)
                {
                    TempData["ErrorMessage"] = "You must select a valid payment method.";
                    return View(model);
                }
                if (model.Purchase.UserId == "Unselected")
                {
                    TempData["ErrorMessage"] = "You must select a valid user.";
                    return View(model);
                }
                // Save the purchase first to obtain an id
                _context.Purchases.Add(model.Purchase);
                await _context.SaveChangesAsync();

                foreach (var detail in model.PurchaseDetails)
                {
                    detail.PurchaseId = model.Purchase.Id;
                    _context.PurchaseDetails.Add(detail);
                    //Add products to stock
                    await _productService.addStockAsync(detail.ProductId, detail.Quantity);
                }
                await _context.SaveChangesAsync();
                // Update total price of the purchase
                await _purchaseService.UpdatePurchaseTotalAsync(model.Purchase.Id);
                TempData["SuccessMessage"] = "Purchase created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while creating the purchase. Please try again.";
                return View(model);
            }
        }

        ///TEST -----------------------------------------------------
        // GET: Purchase/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var purchase = await _context.Purchases.FindAsync(id);
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                return NotFound();
            }

            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            ViewData["PaymentMethodsId"] = new SelectList(_context.PaymentMethods, "Id", "Name");
            return View(purchase);
        }

        // POST: Purchase/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Total,Date,SupplierId,UserId,PaymentMethodId")] Purchase purchase)
        {
            if (id != purchase.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(purchase);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Purchase updated successfully";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while updating the purchase";
                    Console.WriteLine(ex.Message);
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            ViewData["PaymentMethodsId"] = new SelectList(_context.PaymentMethods, "Id", "Name");
            return View(purchase);
        }

        // GET: Purchase/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases
                .Include(p => p.PaymentMethod)
                .Include(p => p.Supplier)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }

        // POST: Purchase/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase != null)
            {
                _context.Purchases.Remove(purchase);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseExists(int id)
        {
            return _context.Purchases.Any(e => e.Id == id);
        }
        // GET: Purchase/GetProductPrice
        [HttpGet]
        public IActionResult GetProductPrice(int productId)
        {
            // Buscar el producto en la base de datos
            var product = _context.Products.Find(productId);

            // Devolver el precio (o 0 si no existe)
            return Json(new { price = product?.Price ?? 0 });
        }
    } 
}
