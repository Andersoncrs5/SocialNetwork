using Microsoft.AspNetCore.Identity;
using SocialNetwork.Write.API.Modules.Role.Model;

namespace SocialNetwork.Write.API.Modules.Role.Repository.Interfaces;

public interface IRoleRepository
{
    Task<IEnumerable<RoleModel>> GetAllAsync();
    Task<RoleModel?> GetByIdAsync(string id);
    Task<bool> ExistsByIdAsync(string id);
    Task<RoleModel?> GetByNameAsync(string name);
    Task<bool> ExistsByNameAsync(string name);
    Task<IdentityResult> CreateAsync(RoleModel role);
    Task<IdentityResult> UpdateAsync(RoleModel role);
    Task<IdentityResult> DeleteAsync(RoleModel role);
}