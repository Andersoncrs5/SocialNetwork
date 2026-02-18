using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.Modules.Role.Model;
using SocialNetwork.Write.API.Modules.Role.Service.Interface;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Write.API.Modules.Role.Service.Provider;

public class RoleService(IUnitOfWork uow): IRoleService
{
    public async Task<RoleModel> GetByNameSimple(string name, TimeSpan? ttl = null)
    {
        RoleModel? model = await uow.RedisService.GetAsync<RoleModel>(name);
        
        if (model != null) 
            return model;
        
        RoleModel role =  await uow.RoleRepository.GetByNameAsync(name) ?? throw new ModelNotFoundException($"Role {name} not found");
        await uow.RedisService.CreateAsync(name, role, ttl);
        
        return role;
    }
    
}