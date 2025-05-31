using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PuntoDeVentaWeb.Data;
using PuntoDeVentaWeb.Models;

namespace PuntoDeVentaWeb.Controllers
{
    public class PurchaseDetailController : Controller
    {
        private readonly DataContext _context;
        private readonly IPurchaseService _purchaseService;

        public PurchaseDetailController(DataContext context, IPurchaseService purchaseService)
        {
            _context = context;
            _purchaseService = purchaseService;
        }

        // GET: PurchaseDetail/Index/purchaseId
        public async Task<IActionResult> Index(int? purchaseId)
        {
            // If receive a purchase id, filter the purchase details by that id.
            if (purchaseId != null)
            {
                var purchaseDetails = _context.PurchaseDetails
                    .Include(p => p.Product)
                    .Include(p => p.Purchase)
                    .Where(p => p.PurchaseId == purchaseId);

                ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
                ViewData["PurchaseId"] = new SelectList(_context.Purchases, "Id", "Id", purchaseId);

                return View(await purchaseDetails.ToListAsync());
            }
            else // If no purchase id is provided, show all purchase details.
            {
                var purchaseDetails = _context.PurchaseDetails
                    .Include(p => p.Product)
                    .Include(p => p.Purchase)
                    .OrderByDescending(p => p.PurchaseId);

                ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
                ViewData["PurchaseId"] = new SelectList(_context.Purchases, "Id", "Id");

                return View(await purchaseDetails.ToListAsync());
            }
        }

        // GET: PurchaseDetail/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseDetail = await _context.PurchaseDetails
                .Include(p => p.Product)
                .Include(p => p.Purchase)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchaseDetail == null)
            {
                return NotFound();
            }
            ViewData["PurchaseId"] = purchaseDetail.PurchaseId;
            return View(purchaseDetail);
        }

        // GET: PurchaseDetail/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            ViewData["PurchaseId"] = new SelectList(_context.Purchases, "Id", "Id");
            return View();
        }

        // POST: PurchaseDetail/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Quantity,PurchaseId,ProductId")] PurchaseDetail purchaseDetail)
        {
            if (ModelState.IsValid)
            {
                bool checkMultiple = _context.PurchaseDetails
                    .Any(p => p.PurchaseId == purchaseDetail.PurchaseId && p.ProductId == purchaseDetail.ProductId);
                if (checkMultiple)
                {
                    // If the product already exists in the purchase, update the quantity.
                    var existingDetail = _context.PurchaseDetails
                        .FirstOrDefault(p => p.PurchaseId == purchaseDetail.PurchaseId && p.ProductId == purchaseDetail.ProductId);
                    if (existingDetail != null)
                    {
                        existingDetail.Quantity += purchaseDetail.Quantity;
                        _context.Update(existingDetail);
                        // Update the existing detail in the context.
                        await _context.SaveChangesAsync();
                        //Update the total price based on the new quantity.
                        var purchase = await _context.Purchases.FindAsync(existingDetail.PurchaseId);
                        if (purchase != null)
                        {
                            await _purchaseService.UpdatePurchaseTotalAsync(purchaseDetail.PurchaseId);
                        }
                        return RedirectToAction(nameof(Index), new { purchaseId = purchaseDetail.PurchaseId });
                    }
                }
                else
                {
                    // If the product does not exist in the purchase, create a new purchase detail.
                    _context.Add(purchaseDetail);
                    await _context.SaveChangesAsync();
                    // Calculate the total price based on the quantity and product price. And update the purchase total.
                    var purchase = await _context.Purchases.FindAsync(purchaseDetail.PurchaseId);
                    if (purchase != null)
                    {
                        await _purchaseService.UpdatePurchaseTotalAsync(purchaseDetail.PurchaseId);
                    }
                    return RedirectToAction(nameof(Index), new { purchaseId = purchaseDetail.PurchaseId });
                }
                return RedirectToAction(nameof(Index), new { purchaseId = purchaseDetail.PurchaseId });
            }

            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", purchaseDetail.ProductId);
            ViewData["PurchaseId"] = purchaseDetail.PurchaseId;
            return RedirectToAction(nameof(Index), new { purchaseId = purchaseDetail.PurchaseId });
        }

        // GET: PurchaseDetail/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseDetail = await _context.PurchaseDetails.FindAsync(id);
            if (purchaseDetail == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", purchaseDetail.ProductId);
            ViewData["PurchaseId"] = purchaseDetail.PurchaseId;
            return View(purchaseDetail);
        }

        // POST: PurchaseDetail/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Quantity,PurchaseId,ProductId")] PurchaseDetail purchaseDetail)
        {
            if (id != purchaseDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(purchaseDetail);
                    await _context.SaveChangesAsync();
                    // Calculate the total price based on the quantity and product price. And update the purchase total.
                    var purchase = await _context.Purchases.FindAsync(purchaseDetail.PurchaseId);
                    if (purchase != null)
                    {
                        await _purchaseService.UpdatePurchaseTotalAsync(purchaseDetail.PurchaseId);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseDetailExists(purchaseDetail.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", purchaseDetail.ProductId);
            ViewData["PurchaseId"] = purchaseDetail.PurchaseId;
            return View(purchaseDetail);
        }

        // GET: PurchaseDetail/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseDetail = await _context.PurchaseDetails
                .Include(p => p.Product)
                .Include(p => p.Purchase)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchaseDetail == null)
            {
                return NotFound();
            }

            return View(purchaseDetail);
        }

        // POST: PurchaseDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var purchaseDetail = await _context.PurchaseDetails.FindAsync(id);
            if (purchaseDetail != null)
            {
                _context.PurchaseDetails.Remove(purchaseDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseDetailExists(int id)
        {
            return _context.PurchaseDetails.Any(e => e.Id == id);
        }

        private decimal CalculateTotalPrice(PurchaseDetail purchaseDetail)
        {
            var product = _context.Products.Find(purchaseDetail.ProductId);
            if (product != null)
            {
                return purchaseDetail.Quantity * product.Price;
            }
            return 0;
        }
    }
}
