using AutoMapper;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.dto.Category;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Services.Interfaces;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Write.API.Services.Providers;

public class CategoryService(IUnitOfWork uow, IMapper mapper): ICategoryService
{
    public async Task<CategoryModel> GetByIdSimple([IsId] string id)
        => await uow.CategoryRepository.GetByIdAsync(id) ?? throw new ModelNotFoundException("Category not found");
    
    public async Task<CategoryModel?> GetById([IsId] string id) 
        => await uow.CategoryRepository.GetByIdAsync(id);

    public async Task<bool> ExistsById([IsId] string id)
        => await uow.CategoryRepository.ExistsById(id);

    public async Task<CategoryModel?> GetByName(string name)
        => await uow.CategoryRepository.FindByName(name);
    
    public async Task<bool> ExistsByName(string name)
        => await uow.CategoryRepository.ExistsByName(name);
    
    public async Task<bool> ExistsBySlug([SlugConstraint] string slug)
        => await uow.CategoryRepository.ExistsBySlug(slug);
    
    public async Task Delete(CategoryModel category)
    {
        await uow.CategoryRepository.DeleteAsync(category);
        await uow.CommitAsync();
    }

    public async Task<CategoryModel> Create(CreateCategoryDto dto)
    {
        CategoryModel model = mapper.Map<CategoryModel>(dto);

        CategoryModel categoryModel = await uow.CategoryRepository.AddAsync(model);
        await uow.CommitAsync();
        
        return categoryModel;
    }

    public async Task<CategoryModel> Update(UpdateCategoryDto dto, CategoryModel category)
    {
        
        
        CategoryModel update = await uow.CategoryRepository.Update(category);
        await uow.CommitAsync();
        return update;
    }
    
}