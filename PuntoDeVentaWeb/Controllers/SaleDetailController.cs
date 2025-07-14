using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PuntoDeVentaWeb.Data;
using PuntoDeVentaWeb.Models;

namespace PuntoDeVentaWeb.Controllers
{
    public class SaleDetailController : Controller
    {
        private readonly ILogger<SaleDetailController> _logger;
        private readonly DataContext _context;
        private readonly IProductService _productService;

        public SaleDetailController(ILogger<SaleDetailController> logger, DataContext context, IProductService productService)
        {
            _logger = logger;
            _context = context;
            _productService = productService;
        }

        public async Task<IActionResult> Index(int? saleId)
        {
            ViewData["Title"] = "Sale Details";
            if (saleId != null)
            {
                var saleDetails = _context.SaleDetails
                .Include(sd => sd.Product)
                .Include(sd => sd.Sale)
                .Where(sd => sd.SaleId == saleId);
                ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
                ViewData["SaleId"] = new SelectList(_context.Sales, "Id", "Description");
                

                return View(await saleDetails.ToListAsync());
            }
            else
            {
                TempData["ErrorMessage"] = "No details found for the specified sale.";
                return RedirectToAction("Index", "Sale");
            }
        }
        public IActionResult Create()
        {
            var products = _context.Products
                .Where(p => p.Stock > 0)
                .ToList();
            ViewData["ProductId"] = new SelectList(products, "Id", "Name");
            ViewData["SaleId"] = new SelectList(_context.Sales, "Id", "Description");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,SaleId,Quantity")] SaleDetail saleDetail)
        {

            if (saleDetail.Quantity <= 0)
            {
                TempData["ErrorMessage"] = "Quantity must be greater than zero.";
                return RedirectToAction(nameof(Index), new { saleId = saleDetail.SaleId });
            }
            if (ModelState.IsValid)
            {
                bool checkMultiple = _context.SaleDetails
                .Any(sd => sd.ProductId == saleDetail.ProductId && sd.SaleId == saleDetail.SaleId);
                if (checkMultiple)
                {
                    TempData["ErrorMessage"] = "This product is already added to the sale. You can edit the quantity instead.";
                    return RedirectToAction(nameof(Index), new { saleId = saleDetail.SaleId });
                }
                var product = await _context.Products.FindAsync(saleDetail.ProductId);
                if(saleDetail.Quantity > product.Stock)
                {
                    TempData["ErrorMessage"] = "Insufficient stock for the selected product.";
                    return RedirectToAction(nameof(Index), new { saleId = saleDetail.SaleId });
                }
                _context.Add(saleDetail);
                await _context.SaveChangesAsync();
                // Update the stock of the product
                await _productService.removeStockAsync(saleDetail.ProductId, saleDetail.Quantity);
                
                TempData["SuccessMessage"] = "Detail correctly added.";
                return RedirectToAction(nameof(Index), new { saleId = saleDetail.SaleId });
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", saleDetail.ProductId);
            ViewData["SaleId"] = new SelectList(_context.Sales, "Id", "Description", saleDetail.SaleId);
            return RedirectToAction(nameof(Index), new { saleId = saleDetail.SaleId });
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}