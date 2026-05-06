using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Modules.PostVote.Dto;
using SocialNetwork.Write.API.Modules.PostVote.Model;
using SocialNetwork.Write.API.Utils.Classes;
using SocialNetwork.Write.API.Utils.result;

namespace SocialNetwork.Write.API.Modules.PostVote.Service.Interface;

public interface IPostVoteService
{
    Task<Result<ResultToggle<PostVoteModel>>> Toggle(
        TogglePostVoteDto dto,
        [IsId] string userId,
        bool commit = true
    );
}