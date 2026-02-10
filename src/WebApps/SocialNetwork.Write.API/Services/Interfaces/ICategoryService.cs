using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.dto.Category;
using SocialNetwork.Write.API.Models;

namespace SocialNetwork.Write.API.Services.Interfaces;

public interface ICategoryService
{
    Task<CategoryModel> GetByIdSimple([IsId] string id);
    Task<CategoryModel?> GetById([IsId] string id);
    Task Delete(CategoryModel category, bool commit = true);
    Task<bool> ExistsById([IsId] string id);
    Task<CategoryModel> Create(CreateCategoryDto dto, bool commit = true);
    Task<CategoryModel?> GetByName(string name);
    Task<bool> ExistsByName(string name);
    Task<bool> ExistsBySlug([SlugConstraint] string slug);
    Task<CategoryModel> Update(UpdateCategoryDto dto, CategoryModel category, bool commit = true);

}