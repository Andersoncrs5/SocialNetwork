using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Write.API.Modules.Role.Model;
using SocialNetwork.Write.API.Modules.Role.Repository.Interfaces;

namespace SocialNetwork.Write.API.Modules.Role.Repository.Provider;

public class RoleRepository(RoleManager<RoleModel> roleManager) : IRoleRepository
{
    public async Task<IEnumerable<RoleModel>> GetAllAsync()
        => await roleManager.Roles.ToListAsync();

    public async Task<RoleModel?> GetByIdAsync(string id)
        => await roleManager.FindByIdAsync(id);

    public async Task<bool> ExistsByIdAsync(string id)
        => await roleManager.FindByIdAsync(id) != null; 
    
    public async Task<RoleModel?> GetByNameAsync(string name)
        => await roleManager.FindByNameAsync(name);
    
    public async Task<bool> ExistsByNameAsync(string name)
        => await roleManager.FindByNameAsync(name) != null;

    public async Task<IdentityResult> CreateAsync(RoleModel role)
        => await roleManager.CreateAsync(role);

    public async Task<IdentityResult> UpdateAsync(RoleModel role)
        => await roleManager.UpdateAsync(role);

    public async Task<IdentityResult> DeleteAsync(RoleModel role)
        => await roleManager.DeleteAsync(role);
        
}