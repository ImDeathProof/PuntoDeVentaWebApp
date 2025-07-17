using PuntoDeVentaWeb.Models;

public interface IClientRepository
{
    Task AddClientAsync(Client client);
    Task UpdateClientAsync(Client client);
    Task DeleteClientAsync(Client client);
    Task<IEnumerable<Client>> GetAllClientsAsync();
    Task<Client> GetClientByIdAsync(int id);
    Task<bool> ClientExistsAsync(string name);
    Task<bool> ClientExistsAsync(int id);
    Task<bool> ClientEmailExistsAsync(string email);
    Task<IEnumerable<Client>> FilterClientsAsync(string filter);

    
}