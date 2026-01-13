using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Write.API.Configs.DB;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Repositories.Interfaces;

namespace SocialNetwork.Write.API.Repositories.Provider;

public class UserRepository(AppDbContext context, UserManager<UserModel> manager): IUserRepository
{
    public async Task<UserModel?> GetByIdAsync(string id)
    {
        return await manager.FindByIdAsync(id);
    }

    public async Task<bool> ExistsByIdAsync(string id)
    {
        return await manager.FindByIdAsync(id) != null;
    }
    
    public async Task<UserModel?> GetByEmail(string email)
    {
        return await manager.FindByEmailAsync(email);
    }

    public async Task<bool> ExistsByEmail(string email)
    {
        return await manager.FindByEmailAsync(email) != null;
    }
    
    public async Task<bool> CheckPassword(UserModel user, string password)
    {
        return await manager.CheckPasswordAsync(user, password);
    }
    
    public async Task<UserModel?> GetByRefreshToken(string refreshToken)
    {
        UserModel? user = await context.Users.
            Where(u => 
                u.RefreshToken == refreshToken&& 
                u.RefreshTokenExpiryTime > DateTime.UtcNow
            ).
            FirstOrDefaultAsync();
        
        return user;
    }
    
    public async Task<bool> ExistsByUsername(string username)
    {
        var user = await manager.FindByNameAsync(username);
        return user != null;
    }
    
    public async Task<UserModel?> GetByUsername(string username)
    {
        return await manager.FindByNameAsync(username);
    }
    
    public async Task<IdentityResult> Delete(UserModel user)
    {
        return await manager.DeleteAsync(user);
    }

    public async Task<IdentityResult> Insert(UserModel user)
    {
        user.CreatedAt = DateTime.UtcNow;
        return await manager.CreateAsync(user, user.PasswordHash!);
    }

    public async Task<IdentityResult> Update(UserModel user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        return await manager.UpdateAsync(user);
    }
    
    public async Task<IdentityResult> AddRoleToUser(UserModel user, RoleModel role)
    {
        return await manager.AddToRoleAsync(user, role.Name!);
    }
    
    public async Task<IdentityResult> RemoveRoleToUser(UserModel user, string roleName)
    {
        return await manager.RemoveFromRoleAsync(user, roleName);
    }

    public async Task<IList<string>> GetRolesAsync(UserModel user)
    {
        IList<string> rolesAsync = await manager.GetRolesAsync(user);
        return rolesAsync;
    }

    public IQueryable<UserModel> GetIQueryable()
    {
        return context.Users.AsQueryable();
    }
    
}