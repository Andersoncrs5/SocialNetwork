using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Modules.PostReactions.Model;
using SocialNetwork.Write.API.Repositories.Interfaces;

namespace SocialNetwork.Write.API.Modules.PostReactions.Repository.Interface;

public interface IPostReactionRepository: IGenericRepository<PostReactionModel>
{
    Task<PostReactionModel?> GetByUserIdAndPostId([IsId] string userId, [IsId] string postId);
}