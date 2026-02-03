using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.dto.Tag;
using SocialNetwork.Write.API.Models;

namespace SocialNetwork.Write.API.Services.Interfaces;

public interface ITagService
{
    Task<TagModel> GetByIdSimpleAsync([IsId] string id);
    Task<bool> ExistsByIdAsync([IsId] string id);
    Task<bool> ExistsByNameAsync(string name);
    Task<TagModel> CreateAsync(CreateTagDto dto);
}