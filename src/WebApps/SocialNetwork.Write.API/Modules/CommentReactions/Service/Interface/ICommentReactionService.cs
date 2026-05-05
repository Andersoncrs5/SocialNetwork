using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Modules.CommentReactions.Dto;
using SocialNetwork.Write.API.Modules.CommentReactions.Model;
using SocialNetwork.Write.API.Utils.Classes;

namespace SocialNetwork.Write.API.Modules.CommentReactions.Service.Interface;

public interface ICommentReactionService
{
    Task<ResultToggle<CommentReactionModel?>> ToggleAsync(ToggleCommentReactionDto dto, [IsId] string userId,
        bool commit = true);
}