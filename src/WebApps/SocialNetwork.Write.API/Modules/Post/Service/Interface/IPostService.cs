using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Modules.Post.Dto;
using SocialNetwork.Write.API.Modules.Post.Model;

namespace SocialNetwork.Write.API.Modules.Post.Service.Interface;

public interface IPostService
{
    Task<PostModel?> GetByIdAsync([IsId] string id);
    Task<PostModel> GetByIdSimpleAsync([IsId] string id);
    Task<bool> ExistsByIdAsync([IsId] string id);
    Task<bool> ExistsBySlugAsync([SlugConstraint] string slug);
    Task<PostModel> CreateAsync(CreatePostDto dto, UserModel user, bool commit = true);
    Task DeleteAsync(PostModel post, bool commit = true);
    Task<PostModel> UpdateAsync(PostModel post, UpdatePostDto dto, bool commit = true);
}