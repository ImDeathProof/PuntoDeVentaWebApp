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
        private readonly ISaleService _saleService;

        public SaleDetailController(ILogger<SaleDetailController> logger, DataContext context, IProductService productService, ISaleService saleService)
        {
            _logger = logger;
            _context = context;
            _productService = productService;
            _saleService = saleService;
        }

        public async Task<IActionResult> Index(int? saleId)
        {
            try
            {
                if (saleId == null)
                {
                    TempData["ErrorMessage"] = "No details found for the specified sale.";
                    return RedirectToAction("Index", "Sale");
                }
                var saleDetails = await _saleService.GetSaleDetailsAsync(saleId.Value);
                ViewData["ProductId"] = await _productService.GetProductSelectListAsync();
                ViewData["SaleId"] = new SelectList(_context.Sales, "Id", "Description");
                return View(saleDetails);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching sale details for Sale ID {SaleId}", saleId);
                TempData["ErrorMessage"] = "An error occurred while trying to fetch the sale details.";
                return RedirectToAction("Index", "Sale");
            }
        }
        public async Task<IActionResult> Create()
        {
            ViewData["ProductId"] = await _productService.GetProductSelectListAsync();
            ViewData["SaleId"] = new SelectList(_context.Sales, "Id", "Description");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,SaleId,Quantity")] SaleDetail saleDetail)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", saleDetail.ProductId);
                    ViewData["SaleId"] = new SelectList(_context.Sales, "Id", "Description", saleDetail.SaleId);
                    TempData["ErrorMessage"] = "Something went wrong, please try again.";
                    return RedirectToAction(nameof(Index), new { saleId = saleDetail.SaleId });
                }
                var product = await _context.Products.FindAsync(saleDetail.ProductId);

                if (product == null)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    return RedirectToAction(nameof(Index), new { saleId = saleDetail.SaleId });
                }
                //check if product has enough stock
                if (product.Stock < saleDetail.Quantity)
                {
                    TempData["ErrorMessage"] = "Insufficient stock for the selected product.";
                    return RedirectToAction(nameof(Index), new { saleId = saleDetail.SaleId });
                }
                if (await _saleService.CheckIfProductExistsInSaleAsync(saleDetail.ProductId, saleDetail.SaleId))
                {
                    TempData["ErrorMessage"] = "This product is already added to the sale.";
                    return RedirectToAction(nameof(Index), new { saleId = saleDetail.SaleId });
                }
                //remove stock from product
                await _productService.removeStockAsync(saleDetail.ProductId, saleDetail.Quantity);
                //add sale detail
                await _saleService.AddSaleDetailAsync(saleDetail);//Sale total is updated in the service method
                TempData["SuccessMessage"] = "Sale detail added successfully.";
                return RedirectToAction(nameof(Index), new { saleId = saleDetail.SaleId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating sale detail for Sale ID {SaleId}", saleDetail.SaleId);
                TempData["ErrorMessage"] = "An error occurred while trying to add the sale detail.";
                ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", saleDetail.ProductId);
                ViewData["SaleId"] = new SelectList(_context.Sales, "Id", "Description", saleDetail.SaleId);
                return RedirectToAction(nameof(Index), new { saleId = saleDetail.SaleId });
            }
        }
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var saleDetail = await _saleService.GetSaleDetailByIdAsync(id.Value);
                if (saleDetail == null)
                {
                    return NotFound();
                }
                ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", saleDetail.ProductId);
                ViewData["SaleId"] = new SelectList(_context.Sales, "Id", "Id", saleDetail.SaleId);
                return View(saleDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while editing sale detail with ID {Id}", id);
                TempData["ErrorMessage"] = "An error occurred while trying to edit the sale detail.";
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,SaleId,Quantity")] SaleDetail saleDetail)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", saleDetail.ProductId);
                    ViewData["SaleId"] = new SelectList(_context.Sales, "Id", "Description", saleDetail.SaleId);
                    TempData["ErrorMessage"] = "Something went wrong, please try again.";
                    return View(saleDetail);
                }
                if (id != saleDetail.Id)
                {
                    return NotFound();
                }
                var existingSaleDetail = await _saleService.GetSaleDetailAsync(saleDetail.SaleId, saleDetail.Id);
                if (existingSaleDetail == null)
                {
                    TempData["ErrorMessage"] = "Sale detail not found.";
                    return RedirectToAction(nameof(Index), new { saleId = saleDetail.SaleId });
                }
                //check product stock and new quantity
                var product = await _context.Products.FindAsync(saleDetail.ProductId);
                if (product == null)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    return RedirectToAction(nameof(Index), new { saleId = saleDetail.SaleId });
                }
                int quantityDifference;
                if (saleDetail.Quantity > existingSaleDetail.Quantity)
                {
                    quantityDifference = saleDetail.Quantity - existingSaleDetail.Quantity;
                    //check if product has enough stock
                    if (product.Stock < quantityDifference)
                    {
                        TempData["ErrorMessage"] = "Insufficient stock for the selected product.";
                        return RedirectToAction(nameof(Index), new { saleId = saleDetail.SaleId });
                    }
                    //update product stock - remove stock
                    await _productService.removeStockAsync(saleDetail.ProductId, quantityDifference);
                }
                else if (saleDetail.Quantity < existingSaleDetail.Quantity)
                {
                    quantityDifference = existingSaleDetail.Quantity - saleDetail.Quantity;
                    //update product stock - add stock
                    await _productService.addStockAsync(saleDetail.ProductId, quantityDifference);
                }

                existingSaleDetail.Quantity = saleDetail.Quantity;
                existingSaleDetail.ProductId = saleDetail.ProductId;
                existingSaleDetail.SaleId = saleDetail.SaleId;
                //Sale total is updated after the sale detail is updated in the next step
                await _saleService.UpdateSaleDetailAsync(existingSaleDetail);
                TempData["SuccessMessage"] = "Sale detail updated successfully.";
                return RedirectToAction(nameof(Index), new { saleId = saleDetail.SaleId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating sale detail with ID {Id}", id);
                TempData["ErrorMessage"] = "An error occurred while trying to update the sale detail.";
                return RedirectToAction(nameof(Index), new { saleId = saleDetail.SaleId });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    TempData["ErrorMessage"] = "Id cannot be null";
                    return NotFound();
                }
                var saleDetail = await _saleService.GetSaleDetailByIdAsync(id.Value);
                if(saleDetail == null)
                {
                    TempData["ErrorMessage"] = "Sale detail not found.";
                    return RedirectToAction(nameof(Index));
                }
                //restore product stock
                await _productService.addStockAsync(saleDetail.ProductId, saleDetail.Quantity);
                //delete sale detail
                await _saleService.DeleteSaleDetailAsync(saleDetail);
                TempData["SuccessMessage"] = "Sale detail deleted successfully.";
                return RedirectToAction(nameof(Index), new { saleId = saleDetail.SaleId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating sale detail with ID {Id}", id);
                TempData["ErrorMessage"] = "An error occurred while trying to delete the sale detail." + ex;
                return RedirectToAction(nameof(Index));
            }

        }
    
    }
}