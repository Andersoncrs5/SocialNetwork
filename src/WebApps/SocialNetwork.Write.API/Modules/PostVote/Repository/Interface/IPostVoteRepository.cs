using SocialNetwork.Write.API.Modules.PostVote.Model;
using SocialNetwork.Write.API.Repositories.Interfaces;

namespace SocialNetwork.Write.API.Modules.PostVote.Repository.Interface;

public interface IPostVoteRepository: IGenericRepository<PostVoteModel>
{
    Task<PostVoteModel?> GetByPostIdAndUserId(string postId, string userId);
}