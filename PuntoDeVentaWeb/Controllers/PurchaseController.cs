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
    public class PurchaseController : Controller
    {
        private readonly DataContext _context;

        public PurchaseController(DataContext context)
        {
            _context = context;
        }

        // GET: Purchase
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.Purchases.Include(p => p.PaymentMethod).Include(p => p.Supplier).Include(p => p.User);
            return View(await dataContext.ToListAsync());
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



        // // POST: Purchase/Create
        // // To protect from overposting attacks, enable the specific properties you want to bind to.
        // // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        public IActionResult Create()
        {
            var model = new PurchaseViewModel
            {
                Purchase = new Purchase(),
                PurchaseDetails = new List<PurchaseDetail> { new PurchaseDetail() } // 1 fila vacía inicial
            };
            ViewBag.Suppliers = new SelectList(_context.Suppliers, "Id", "Name");
            ViewBag.Users = new SelectList(_context.Users, "Id", "Name");
            ViewBag.Products = new SelectList(_context.Products, "Id", "Name");
            ViewBag.PaymentMethods = new SelectList(_context.PaymentMethods, "Id", "Name");
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Loggear errores de validación
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View(model);
            }
            try
            {
                // Guardar Purchase (maestro)
                _context.Purchases.Add(model.Purchase);
                _context.SaveChanges(); // Guardar PurchaseDetails (detalles)

                if (model.PurchaseDetails?.Count > 0)
                {
                    foreach (var detail in model.PurchaseDetails)
                    {
                        detail.PurchaseId = model.Purchase.Id; // Asignar FK
                        _context.PurchaseDetails.Add(detail);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError("", "No se pudo guardar la compra.");
                //Recargar los views para que se muestren nuevamente los selectlists
                ViewBag.Suppliers = new SelectList(_context.Suppliers, "Id", "Name");
                ViewBag.Users = new SelectList(_context.Users, "Id", "Name");
                ViewBag.Products = new SelectList(_context.Products, "Id", "Name");
                ViewBag.PaymentMethods = new SelectList(_context.PaymentMethods, "Id", "Name");
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

            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                return NotFound();
            }
            ViewData["PaymentMethodId"] = new SelectList(_context.Payments, "Id", "Name", purchase.PaymentMethodId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", purchase.SupplierId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", purchase.UserId);
            return View(purchase);
        }

        // POST: Purchase/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Total,Date,SupplierId,UserId,PaymentId")] Purchase purchase)
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
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseExists(purchase.Id))
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
            ViewData["PaymentMethodId"] = new SelectList(_context.Payments, "Id", "Name", purchase.PaymentMethodId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", purchase.SupplierId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", purchase.UserId);
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
