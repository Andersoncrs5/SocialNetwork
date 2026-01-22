using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Configs.DB;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Repositories.Interfaces;

namespace SocialNetwork.Write.API.Repositories.Provider;

public class UserRepository(AppDbContext context, UserManager<UserModel> manager): IUserRepository
{
    public Task<UserModel?> GetByIdAsync(string id)
        => manager.FindByIdAsync(id);

    public async Task<bool> ExistsByIdAsync(string id)
        => await manager.FindByIdAsync(id) != null;

    public Task<UserModel?> GetByEmail(string email)
        => manager.FindByEmailAsync(email);

    public async Task<bool> ExistsByEmail(string email)
        => await manager.FindByEmailAsync(email) != null;

    public Task<UserModel?> GetByUsername(string username)
        => manager.FindByNameAsync(username);

    public async Task<bool> ExistsByUsername(string username)
        => await manager.FindByNameAsync(username) != null;

    public Task<bool> CheckPassword(UserModel user, string password)
        => manager.CheckPasswordAsync(user, password);

    public async Task<UserModel?> GetByRefreshToken(string refreshToken)
    {
        return await manager.Users
            .Where(u =>
                u.RefreshToken == refreshToken &&
                u.RefreshTokenExpiryTime > DateTime.UtcNow
            )
            .FirstOrDefaultAsync();
    }

    public Task<IdentityResult> Insert(UserModel user)
    {
        user.CreatedAt = DateTime.UtcNow;
        return manager.CreateAsync(user, user.PasswordHash!);
    }

    public Task<IdentityResult> Update(UserModel user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        return manager.UpdateAsync(user);
    }
    
    public Task<IdentityResult> Delete(UserModel user)
        => manager.DeleteAsync(user);

    public Task<IdentityResult> AddRoleToUser(UserModel user, RoleModel role)
        => manager.AddToRoleAsync(user, role.Name!);

    public Task<IdentityResult> RemoveRoleToUser(UserModel user, string roleName)
        => manager.RemoveFromRoleAsync(user, roleName);

    public Task<IList<string>> GetRolesAsync(UserModel user)
        => manager.GetRolesAsync(user);

}