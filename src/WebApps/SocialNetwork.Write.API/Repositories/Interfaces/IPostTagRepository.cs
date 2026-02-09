using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Models;

namespace SocialNetwork.Write.API.Repositories.Interfaces;

public interface IPostTagRepository: IGenericRepository<PostTagModel>
{
    Task<int> CountByPostIdAndTagId([IsId] string postId, [IsId] string tagId);
    Task<PostTagModel?> GetByPostIdAndTagId([IsId] string postId, [IsId] string tagId);
}