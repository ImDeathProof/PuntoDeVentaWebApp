using PuntoDeVentaWeb.Models;

public interface IClientService
{
    Task<IEnumerable<Client>> GetAllClientsAsync();
    Task<Client> GetClientByIdAsync(int id);
    Task AddClientAsync(Client client);
    Task UpdateClientAsync(Client client);
    Task DeleteClientAsync(Client client);
    Task<bool> ClientExistsAsync(string name);
    Task<bool> ClientExistsAsync(int id);
    Task<IEnumerable<Client>> FilterClientsAsync(string filter);
}