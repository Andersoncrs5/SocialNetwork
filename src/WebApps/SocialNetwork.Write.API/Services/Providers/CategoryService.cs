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
        category.Description = dto.Description ?? category.Description;
        category.IconName = dto.IconName ?? category.IconName;
        category.Color = dto.Color ?? category.Color;
        if (dto.IsActive.HasValue) category.IsActive = dto.IsActive.Value;
        if (dto.IsVisible.HasValue) category.IsVisible = dto.IsVisible.Value;
        if (dto.DisplayOrder.HasValue) category.DisplayOrder = dto.DisplayOrder.Value;
        
        if (!string.IsNullOrWhiteSpace(dto.Name) && category.Name != dto.Name)
        {
            if (await uow.CategoryRepository.ExistsByName(dto.Name))
                throw new ConflictValueException($"Name: {dto.Name} already in use"); 
             
            category.Name = dto.Name;
        }
        
        if (!string.IsNullOrWhiteSpace(dto.Slug) && category.Slug != dto.Slug)
        {
            if (await uow.CategoryRepository.ExistsBySlug(dto.Slug))
                throw new ConflictValueException($"Slug: {dto.Slug} already in use"); 
            
            category.Slug = dto.Slug;
        }
        
        if (!string.IsNullOrWhiteSpace(dto.ParentId) && category.ParentId != dto.ParentId)
        {
            if (dto.ParentId == category.Id)
                throw new SelfReferencingHierarchyException("A category cannot be its own parent.");
            
            if (await uow.CategoryRepository.ExistsById(dto.ParentId) == false)
                throw new ModelNotFoundException("Parent not found");
            
            category.ParentId = dto.ParentId;
        }
        
        if (dto.BecameRoot.HasValue && dto.BecameRoot.Value) category.ParentId = null;
        
        CategoryModel update = await uow.CategoryRepository.Update(category);
        await uow.CommitAsync();
        return update;
    }
    
}