using AutoMapper;
using SocialNetwork.Contracts.DTOs.Comment;
using SocialNetwork.Write.API.dto.Comment;
using SocialNetwork.Write.API.Models;

namespace SocialNetwork.Write.API.Utils.Mappers;

public class CommentMapper: Profile
{
    public CommentMapper()
    {
        CreateMap<CreateCommentDto, CommentModel>();
        CreateMap<CommentModel, CommentDto>().ReverseMap();
    }
}