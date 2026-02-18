using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Modules.Category.Model;
using SocialNetwork.Write.API.Repositories.Interfaces;

namespace SocialNetwork.Write.API.Modules.Category.Repository.Interface;

public interface ICategoryRepository: IGenericRepository<CategoryModel>
{
    Task<CategoryModel?> FindBySlug([SlugConstraint] string slug);
    Task<bool> ExistsBySlug([SlugConstraint] string slug);
    Task<CategoryModel?> FindByName(string name);
    Task<bool> ExistsByName(string name);
}