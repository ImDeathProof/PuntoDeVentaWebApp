using System.CodeDom;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using PuntoDeVentaWeb.Models;
public class ClientService : IClientService
{
    private readonly IClientRepository _repository;
    public ClientService(IClientRepository repository)
    {
        _repository = repository;
    }
    public async Task AddClientAsync(Client client)
    {
        try
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client), "Client cannot be null");
            }
            if (await _repository.ClientEmailExistsAsync(client.Email))
            {
                throw new InvalidOperationException("Client with this email already exists");
            }
            await _repository.AddClientAsync(client);
        }
        catch (ArgumentNullException)
        {
            throw new ClientServiceException("Client cannot be null");
        }
        catch (InvalidOperationException)
        {
            throw new ClientServiceException("Client with this email already exists");
        }
        catch (DbUpdateException dbEx)
        {
            throw new ClientServiceException("An error occurred while updating the database", dbEx);
        }
        catch (Exception ex)
        {
            throw new ClientServiceException("An error occurred while adding the client", ex);
        }
    }

    public async Task<bool> ClientExistsAsync(string name)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Client name cannot be null or empty", nameof(name));
            }
            return await _repository.ClientExistsAsync(name);
        }
        catch (ArgumentException)
        {
            throw new ClientServiceException("Client name cannot be null or empty");
        }
        catch (Exception ex)
        {
            throw new ClientServiceException("An error occurred while checking if the client exists", ex);
        }
    }

    public async Task<bool> ClientExistsAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                throw new ArgumentException("Client ID must be greater than zero", nameof(id));
            }
            return await _repository.ClientExistsAsync(id);
        }
        catch (ArgumentException)
        {
            throw new ClientServiceException("Client ID must be greater than zero");
        }
        catch (Exception ex)
        {
            throw new ClientServiceException("An error occurred while checking if the client exists", ex);
        }
    }

    public async Task DeleteClientAsync(Client client)
    {
        try
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client), "Client cannot be null");
            }
            await _repository.DeleteClientAsync(client);
        }
        catch (ArgumentNullException)
        {
            throw new ClientServiceException("Client cannot be null");
        }
        catch (DbUpdateException dbEx)
        {
            throw new ClientServiceException("An error occurred while updating the database. May be the client is relationed with other records", dbEx);
        }
        catch (Exception ex)
        {
            throw new ClientServiceException("An error occurred while deleting the client", ex);
        }
    }

    public async Task<IEnumerable<Client>> FilterClientsAsync(string filter)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                throw new ArgumentException("Filter cannot be null or empty", nameof(filter));
            }
            return await _repository.FilterClientsAsync(filter);
        }
        catch (ArgumentException)
        {
            throw new ClientServiceException("Filter cannot be null or empty");
        }
        catch (DbException dbEx)
        {
            throw new ClientServiceException("An error occurred while reading the database", dbEx);
        }
        catch (Exception ex)
        {
            throw new ClientServiceException("An error occurred while filtering clients", ex);
        }
    }

    public async Task<IEnumerable<Client>> GetAllClientsAsync()
    {
        try
        {
            return await _repository.GetAllClientsAsync();
        }
        catch (InvalidOperationException)
        {
            throw new ClientServiceException("No clients found in the database.");
        }
        catch (DbUpdateException dbEx)
        {
            throw new ClientServiceException("An error occurred while updating the database", dbEx);
        }
        catch (Exception ex)
        {
            throw new ClientServiceException("An error occurred while retrieving all clients", ex);
        }
    }

    public async Task<Client> GetClientByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                throw new ArgumentException("Client ID must be greater than zero", nameof(id));
            }
            return await _repository.GetClientByIdAsync(id);
        }
        catch (ArgumentException)
        {
            throw new ClientServiceException("Client ID must be greater than zero");
        }
        catch (InvalidOperationException)
        {
            throw new ClientServiceException("Client not found with the provided ID.");
        }
        catch (DbUpdateException dbEx)
        {
            throw new ClientServiceException("An error occurred while updating the database", dbEx);
        }
        catch (Exception ex)
        {
            throw new ClientServiceException("An error occurred while retrieving the client by ID", ex);
        }
    }

    public Task UpdateClientAsync(Client client)
    {
        try
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client), "Client cannot be null");
            }
            return _repository.UpdateClientAsync(client);
        }
        catch (ArgumentNullException)
        {
            throw new ClientServiceException("Client cannot be null");
        }
        catch (DbUpdateException dbEx)
        {
            throw new ClientServiceException("An error occurred while updating the database", dbEx);
        }
        catch (Exception ex)
        {
            throw new ClientServiceException("An error occurred while updating the client", ex);
        }
    }
}
public class ClientServiceException : Exception
{
    public ClientServiceException(string message) : base(message) { }
    public ClientServiceException(string message, Exception innerException) : base(message, innerException) { }
}