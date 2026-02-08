using SocialNetwork.Write.API.Models;

namespace SocialNetwork.Write.API.Services.Interfaces;

public interface IRoleService
{
    Task<RoleModel> GetByNameSimple(string name, TimeSpan? ttl = null);
}