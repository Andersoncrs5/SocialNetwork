using Microsoft.AspNetCore.Identity;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Modules.Role.Model;
using SocialNetwork.Write.API.Modules.User.Model;

namespace SocialNetwork.Write.API.Modules.User.Repository.Interface;

public interface IUserRepository
{
    Task<UserModel?> GetByIdAsync([IsId] string id);
    Task<bool> ExistsByIdAsync([IsId] string id);
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
    Task<bool> IsInRoleAsync(UserModel user, string roleName);
}