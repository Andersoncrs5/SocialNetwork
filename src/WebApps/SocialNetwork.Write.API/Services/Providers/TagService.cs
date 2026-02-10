using AutoMapper;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.dto.Tag;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Services.Interfaces;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Write.API.Services.Providers;

public class TagService(IUnitOfWork uow, IMapper mapper): ITagService
{
    public async Task<TagModel> GetByIdSimpleAsync([IsId] string id)
        => await uow.TagRepository.GetByIdAsync(id) ?? throw new ModelNotFoundException("Tag not found");

    public async Task<bool> ExistsByIdAsync([IsId] string id)
        => await uow.TagRepository.ExistsById(id);

    public async Task<bool> ExistsByNameAsync(string name)
        => await uow.TagRepository.ExistsByName(name);

    public async Task DeleteAsync(TagModel tag, bool commit = true)
    {
        await uow.TagRepository.DeleteAsync(tag);
        if (commit) await uow.CommitAsync();
    }

    public async Task<TagModel> CreateAsync(CreateTagDto dto, bool commit = true)
    {
        TagModel model = mapper.Map<TagModel>(dto);

        TagModel tagCreated = await uow.TagRepository.AddAsync(model);
        if (commit) await uow.CommitAsync();
        
        return tagCreated;
    }

    public async Task<TagModel> UpdateAsync(TagModel tag, UpdateTagDto dto, bool commit = true)
    {
        if (!string.IsNullOrWhiteSpace(dto.Name) && tag.Name != dto.Name)
        {
            if (await uow.TagRepository.ExistsByName(dto.Name))
                throw new ConflictValueException($"Name: {dto.Name} already in use"); 
            
            tag.Name = dto.Name;
        }
        
        if (!string.IsNullOrWhiteSpace(dto.Slug) && tag.Slug != dto.Slug)
        {
            if (await uow.TagRepository.ExistsBySlug(dto.Slug))
                throw new ConflictValueException($"Slug: {dto.Slug} already in use"); 
            
            tag.Slug = dto.Slug;
        }
        
        if (!string.IsNullOrWhiteSpace(dto.Description))  tag.Description = dto.Description;
        if (!string.IsNullOrWhiteSpace(dto.Color))  tag.Color = dto.Color;
        
        if (dto.IsActive.HasValue) dto.IsActive = dto.IsActive.Value;
        if (dto.IsVisible.HasValue) dto.IsVisible = dto.IsVisible.Value;
        if (dto.IsSystem.HasValue) dto.IsSystem = dto.IsSystem.Value;
        
        TagModel update = await uow.TagRepository.Update(tag);
        if (commit) await uow.CommitAsync();
        return update;
    }
    
}