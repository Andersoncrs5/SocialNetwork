using Microsoft.AspNetCore.Identity;
using SocialNetwork.Write.API.Models;

namespace SocialNetwork.Write.API.Repositories.Interfaces;

public interface IUserRepository
{
    Task<UserModel?> GetByIdAsync(string id);
    Task<bool> ExistsByIdAsync(string id);
    Task<UserModel?> GetByEmail(string email);
    Task<bool> ExistsByEmail(string email);
    Task<bool> CheckPassword(UserModel user, string password);
    Task<UserModel?> GetByRefreshToken(string refreshToken);
    Task<bool> ExistsByUsername(string username);
    Task<UserModel?> GetByUsername(string username);
    Task<IdentityResult> Delete(UserModel user);
    Task<IdentityResult> Insert(UserModel user);
    Task<IdentityResult> Update(UserModel user);
    Task<IdentityResult> AddRoleToUser(UserModel user, RoleModel role);
    Task<IdentityResult> RemoveRoleToUser(UserModel user, string roleName);
    Task<IList<string>> GetRolesAsync(UserModel user);
    IQueryable<UserModel> GetIQueryable();
}