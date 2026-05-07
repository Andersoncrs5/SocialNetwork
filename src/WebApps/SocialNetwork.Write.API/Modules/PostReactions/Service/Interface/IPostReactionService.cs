using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Modules.PostReactions.Dto;
using SocialNetwork.Write.API.Modules.PostReactions.Model;
using SocialNetwork.Write.API.Utils.Classes;
using SocialNetwork.Write.API.Utils.result;

namespace SocialNetwork.Write.API.Modules.PostReactions.Service.Interface;

public interface IPostReactionService
{
    Task<Result<ResultToggle<PostReactionModel?>>> ToggleAsync(
        TogglePostReactionDto dto,
        [IsId] string userId,
        bool commit = true
    );
}