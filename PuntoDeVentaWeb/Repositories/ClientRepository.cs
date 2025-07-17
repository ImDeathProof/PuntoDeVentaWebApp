using Microsoft.EntityFrameworkCore;
using PuntoDeVentaWeb.Data;
using PuntoDeVentaWeb.Models;

public class ClientRepository : IClientRepository
{
    private readonly DataContext _context;
    public ClientRepository(DataContext context)
    {
        _context = context;
    }
    public async Task AddClientAsync(Client client)
    {
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ClientEmailExistsAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be null or empty", nameof(email));
        }
        return await _context.Clients.AnyAsync(c => c.Email.ToLower() == email.ToLower());
    }

    public async Task<bool> ClientExistsAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Client name cannot be null or empty", nameof(name));
        }
        return await _context.Clients.AnyAsync(c => c.Name.ToLower() == name.ToLower());
    }

    public async Task<bool> ClientExistsAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Client ID must be greater than zero", nameof(id));
        }
        return await _context.Clients.AnyAsync(c => c.Id == id);
    }

    public async Task DeleteClientAsync(Client client)
    {
        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Client>> FilterClientsAsync(string filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
        {
            throw new ArgumentException("Filter cannot be null or empty", nameof(filter));
        }
        return await _context.Clients
            .Where(c => c.Name.ToLower().Contains(filter.ToLower()) || 
                        c.Email.ToLower().Contains(filter.ToLower()) ||
                        c.Phone.ToLower().Contains(filter.ToLower()))
            .ToListAsync();
    }

    public async Task<IEnumerable<Client>> GetAllClientsAsync()
    {
        return await _context.Clients.ToListAsync();
    }

    public async Task<Client> GetClientByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Client ID must be greater than zero");
        }
        return await _context.Clients.FindAsync(id) 
            ?? throw new KeyNotFoundException($"Client with ID {id} not found.");
    }

    public async Task UpdateClientAsync(Client client)
    {
        _context.Clients.Update(client);
        await _context.SaveChangesAsync();
    }
}