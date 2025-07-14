using PuntoDeVentaWeb.Models;

public interface IUserService
{
    Task<User> GetUserByIdAsync(string userId);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task AddUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task ActivateUserAsync(string userId);
    Task DesactivateUserAsync(string userId);
    Task<bool> IsUserInRoleAsync(string userId, string roleName);
    Task AddUserToRoleAsync(User user, string roleName);
    Task RemoveUserFromRoleAsync(User user, string roleName);
    Task<string> GetUserRoleAsync(string userId);
}