using AutoMapper;
using SocialNetwork.Contracts.DTOs.CommentReaction;
using SocialNetwork.Write.API.Modules.CommentReactions.Dto;
using SocialNetwork.Write.API.Modules.CommentReactions.Model;

namespace SocialNetwork.Write.API.Utils.Mappers;

public class CommentReactionMapper: Profile
{
    public CommentReactionMapper()
    {
        CreateMap<ToggleCommentReactionDto, CommentReactionModel>();
        CreateMap<CommentReactionModel, CommentReactionDto>().ReverseMap();
    }
}