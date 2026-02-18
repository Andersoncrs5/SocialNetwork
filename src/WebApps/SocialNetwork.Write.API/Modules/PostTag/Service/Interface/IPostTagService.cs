using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Modules.PostTag.Dto;
using SocialNetwork.Write.API.Modules.PostTag.Model;

namespace SocialNetwork.Write.API.Modules.PostTag.Service.Interface;

public interface IPostTagService
{
    Task<PostTagModel> GetByIdAsync([IsId] string id);
    Task<PostTagModel?> GetByPostIdAndTagId([IsId] string postId, [IsId] string tagId);
    Task<int> CountByPostIdAndTagId([IsId] string postId, [IsId] string tagId);
    Task Delete(PostTagModel tag, bool commit = true);
    Task<PostTagModel> Create(CreatePostTagDto dto, bool commit = true);
}