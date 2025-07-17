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
    public class ClientController : Controller
    {
        private readonly DataContext _context;
        private readonly IClientService _clientService;

        public ClientController(DataContext context, IClientService clientService)
        {
            _context = context;
            _clientService = clientService;
        }

        // GET: Client
        public async Task<IActionResult> Index(string search)
        {
            ViewData["CurrentSearch"] = search;
            var clients = await _clientService.GetAllClientsAsync();
            if (!string.IsNullOrEmpty(search))
            {
                clients = await _clientService.FilterClientsAsync(search);
            }
            return View(clients);
        }

        // GET: Client/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                var client = await _clientService.GetClientByIdAsync(id.Value);
                if (client == null)
                {
                    throw new KeyNotFoundException("Client not found");
                }
                return View(client);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching client details: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Client/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Client/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,LastName,Email,Phone,Address")] Client client)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Invalid client data.";
                    return View(client);
                }
                await _clientService.AddClientAsync(client);
                TempData["SuccessMessage"] = "Client created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating client: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
                return View(client);
            }
        }

        // GET: Client/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    TempData["ErrorMessage"] = "Client ID is required.";
                    return RedirectToAction(nameof(Index));
                }

                var client = await _clientService.GetClientByIdAsync(id.Value);
                if (client == null)
                {
                    TempData["ErrorMessage"] = "Client not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(client);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching client for edit: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Client/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,LastName,Email,Phone,Address")] Client client)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Invalid client data.";
                    return View(client);
                }
                if (id != client.Id)
                {
                    return NotFound();
                }
                await _clientService.UpdateClientAsync(client);
                TempData["SuccessMessage"] = "Client updated successfully.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating client: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
            }
            return View(client);
        }

        // GET: Client/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    TempData["ErrorMessage"] = "Client ID is required.";
                    return RedirectToAction(nameof(Index));
                }

                var client = await _clientService.GetClientByIdAsync(id.Value);
                if (client == null)
                {
                    TempData["ErrorMessage"] = "Client not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(client);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error fetching client for deletion: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Client/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            try
            {
                if (id == null)
                {
                    TempData["ErrorMessage"] = "Client ID is required.";
                    return RedirectToAction(nameof(Index));
                }
                var client = await _clientService.GetClientByIdAsync(id.Value);
                if (client == null)
                {
                    TempData["ErrorMessage"] = "Client not found.";
                    return RedirectToAction(nameof(Index));
                }
                await _clientService.DeleteClientAsync(client);
                TempData["SuccessMessage"] = "Client deleted successfully.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting client: {ex.Message}");
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
    }
}
