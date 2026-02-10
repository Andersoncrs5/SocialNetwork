using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.dto.Tag;
using SocialNetwork.Write.API.Models;

namespace SocialNetwork.Write.API.Services.Interfaces;

public interface ITagService
{
    Task<TagModel> GetByIdSimpleAsync([IsId] string id);
    Task<bool> ExistsByIdAsync([IsId] string id);
    Task<bool> ExistsByNameAsync(string name);
    Task<TagModel> CreateAsync(CreateTagDto dto, bool commit = true);
    Task<TagModel> UpdateAsync(TagModel tag, UpdateTagDto dto, bool commit = true);
    Task DeleteAsync(TagModel tag, bool commit = true);
}