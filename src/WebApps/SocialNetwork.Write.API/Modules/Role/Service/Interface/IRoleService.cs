using SocialNetwork.Write.API.Modules.Role.Model;

namespace SocialNetwork.Write.API.Modules.Role.Service.Interface;

public interface IRoleService
{
    Task<RoleModel> GetByNameSimple(string name, TimeSpan? ttl = null);
}