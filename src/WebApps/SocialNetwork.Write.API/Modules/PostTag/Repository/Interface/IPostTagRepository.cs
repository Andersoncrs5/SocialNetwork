using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Modules.PostTag.Model;
using SocialNetwork.Write.API.Repositories.Interfaces;

namespace SocialNetwork.Write.API.Modules.PostTag.Repository.Interface;

public interface IPostTagRepository: IGenericRepository<PostTagModel>
{
    Task<int> CountByPostIdAndTagId([IsId] string postId, [IsId] string tagId);
    Task<PostTagModel?> GetByPostIdAndTagId([IsId] string postId, [IsId] string tagId);
}