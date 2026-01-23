using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Models;

namespace SocialNetwork.Write.API.Repositories.Interfaces;

public interface ICategoryRepository: IGenericRepository<CategoryModel>
{
    Task<CategoryModel?> FindBySlug([SlugConstraint] string slug);
    Task<bool> ExistsBySlug([SlugConstraint] string slug);
    Task<CategoryModel?> FindByName(string name);
    Task<bool> ExistsByName(string name);
}