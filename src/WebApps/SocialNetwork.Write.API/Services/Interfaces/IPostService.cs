using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.dto.Posts;
using SocialNetwork.Write.API.Models;

namespace SocialNetwork.Write.API.Services.Interfaces;

public interface IPostService
{
    Task<PostModel?> GetByIdAsync([IsId] string id);
    Task<bool> ExistsByIdAsync([IsId] string id);
    Task<bool> ExistsBySlugAsync([SlugConstraint] string slug);
    Task<PostModel> CreateAsync(CreatePostDto dto);
    Task DeleteAsync(PostModel post);
}