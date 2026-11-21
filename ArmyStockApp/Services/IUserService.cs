using ArmyStockApp.Models;
namespace ArmyStockApp.Services;
public interface IUserService
{
    Task<bool> CreateAsync(User user);
    Task<User?> LogInCheckAsync(string userName, string password);
    Task<bool> PatchEmailAsync(User user, string newEmail);
    Task<bool> DeleteUserAsync(string email);
    Task<User?> GetByUserNameAsync(string userName);
}
