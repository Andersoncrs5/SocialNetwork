using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Modules.CommentVote.Dto;
using SocialNetwork.Write.API.Modules.CommentVote.Model;
using SocialNetwork.Write.API.Utils.Classes;
using SocialNetwork.Write.API.Utils.result;

namespace SocialNetwork.Write.API.Modules.CommentVote.Service.Interface;

public interface ICommentVoteService
{
    Task<Result<ResultToggle<CommentVoteModel>>> Toggle(
        ToggleCommentVoteDto dto,
        [IsId] string userId,
        bool commit = true
    );
}