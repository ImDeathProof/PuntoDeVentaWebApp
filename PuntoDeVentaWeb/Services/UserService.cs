using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PuntoDeVentaWeb.Models;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<UserRole> _roleManager;
    public UserService(UserManager<User> userManager, RoleManager<UserRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public async Task ActivateUserAsync(string userId)
    {
        try
        {
            if (userId.IsNullOrEmpty())
            {
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new UserServiceException($"User with ID {userId} not found.");
            }
            user.IsActive = true;
            user.DesactivatedDate = null;
            await _userManager.UpdateAsync(user);
        }
        catch (ArgumentException ex)
        {
            throw new UserServiceException("An error occurred while activating the user.", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new UserServiceException("An error occurred while updating the user in the database.", ex);
        }
        catch (Exception ex)
        {
            throw new UserServiceException("An unexpected error occurred while activating the user.", ex);
        }
    }
    public Task AddUserAsync(User user)
    {
        throw new NotImplementedException();
    }

    public Task AddUserToRoleAsync(User user, string roleName)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null.");
        }
        if (roleName.IsNullOrEmpty())
        {
            throw new ArgumentException("Role name cannot be null or empty.", nameof(roleName));
        }
        if (!_roleManager.RoleExistsAsync(roleName).Result)
        {
            throw new UserServiceException($"Role '{roleName}' does not exist.");
        }
        try
        {
            var result = _userManager.AddToRoleAsync(user, roleName).Result;
            if (!result.Succeeded)
            {
                throw new UserServiceException($"Failed to add user {user.UserName} to role {roleName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            return Task.CompletedTask;
        }
        catch (DbUpdateException ex)
        {
            throw new UserServiceException("An error occurred while adding the user to the role in the database.", ex);
        }
        catch (Exception ex)
        {
            throw new UserServiceException("An unexpected error occurred while adding the user to the role.", ex);
        }
    }

    public async Task DesactivateUserAsync(string userId)
    {
        try
        {
            if (userId.IsNullOrEmpty())
            {
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new UserServiceException($"User with ID {userId} not found.");
            }
            user.IsActive = false;
            user.DesactivatedDate = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
        }
        catch (ArgumentException ex)
        {
            throw new UserServiceException("An error occurred while activating the user.", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new UserServiceException("An error occurred while updating the user in the database.", ex);
        }
        catch (Exception ex)
        {
            throw new UserServiceException("An unexpected error occurred while activating the user.", ex);
        }
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        try
        {
            return await _userManager.Users.ToListAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new UserServiceException("An error occurred while retrieving users from the database.", ex);
        }
        catch (Exception ex)
        {
            throw new UserServiceException("An unexpected error occurred while retrieving users.", ex);
        }
    }

    public async Task<User> GetUserByIdAsync(string userId)
    {
        try
        {
            if (userId.IsNullOrEmpty())
            {
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
            }
            var user = await _userManager.FindByIdAsync(userId);
            return user ?? throw new UserServiceException($"User with ID {userId} not found");
        }
        catch (ArgumentException ex)
        {
            throw new UserServiceException("An error occurred while retrieving the user.", ex);
        }
        catch (SqlException  ex)
        {
            throw new UserServiceException("An error occurred while retrieving the user from the database.", ex);
        }
        catch (Exception ex)
        {
            throw new UserServiceException("An unexpected error occurred while retrieving the user.", ex);
        }
    }

    public async Task<string> GetUserRoleAsync(string userId)
    {
        return await _userManager.GetRolesAsync(await GetUserByIdAsync(userId))
            .ContinueWith(task => task.Result.FirstOrDefault() ?? string.Empty);
    }

    public async Task<bool> IsUserInRoleAsync(string userId, string roleName)
    {
        try
        {
            if (userId.IsNullOrEmpty() || roleName.IsNullOrEmpty())
            {
                throw new ArgumentException("User ID and role name cannot be null or empty.");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new UserServiceException($"User with ID {userId} not found.");
            }
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                throw new UserServiceException($"Role '{roleName}' does not exist.");
            }
            return await _userManager.IsInRoleAsync(user, roleName);
        }
        catch (ArgumentException ex)
        {
            throw new UserServiceException("An error occurred while checking if the user is in the specified role.", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new UserServiceException("An error occurred while checking if the user is in the specified role in the database.", ex);
        }
        catch (SqlException ex)
        {
            throw new UserServiceException("An error occurred while checking if the user is in the specified role in the database.", ex);
        }
        catch (Exception ex)
        {
            throw new UserServiceException("An error occurred while checking if the user is in the specified role.", ex);
        }
    }

    public Task RemoveUserFromRoleAsync(User user, string roleName)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null.");
        }
        if (roleName.IsNullOrEmpty())
        {
            throw new ArgumentException("Role name cannot be null or empty.", nameof(roleName));
        }
        try
        {
            var result = _userManager.RemoveFromRoleAsync(user, roleName).Result;
            if (!result.Succeeded)
            {
                throw new UserServiceException($"Failed to remove user {user.UserName} from role {roleName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            return Task.CompletedTask;
        }
        catch (ArgumentNullException ex)
        {
            throw new UserServiceException("An error occurred while removing the user from the role.", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new UserServiceException("An error occurred while removing the user from the role in the database.", ex);
        }
        catch (Exception ex)
        {
            throw new UserServiceException("An unexpected error occurred while removing the user from the role.", ex);
        }
    }

    public async Task UpdateUserAsync(User user)
    {
        try
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }
            var existingUser = await _userManager.FindByIdAsync(user.Id);
            if (existingUser == null)
            {
                throw new UserServiceException($"User with ID {user.Id} not found.");
            }
            existingUser.UserName = user.UserName;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.IsActive = user.IsActive;
            existingUser.DesactivatedDate = user.DesactivatedDate;
            existingUser.Name = user.Name;
            existingUser.LastName = user.LastName;
            await _userManager.UpdateAsync(existingUser);
        }
        catch (ArgumentNullException ex)
        {
            throw new UserServiceException("An error occurred while updating the user.", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new UserServiceException("An error occurred while updating the user in the database.", ex);
        }
        catch (Exception ex)
        {
            throw new UserServiceException("An unexpected error occurred while updating the user.", ex);
        }
    }
}
public class UserServiceException : Exception
{
    public UserServiceException(string message) : base(message)
    {
    }

    public UserServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }
}