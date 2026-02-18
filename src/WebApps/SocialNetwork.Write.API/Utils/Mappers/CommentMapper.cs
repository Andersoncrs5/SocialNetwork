using AutoMapper;
using SocialNetwork.Contracts.DTOs.Comment;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Modules.Comment.Dto;
using SocialNetwork.Write.API.Modules.Comment.Model;

namespace SocialNetwork.Write.API.Utils.Mappers;

public class CommentMapper: Profile
{
    public CommentMapper()
    {
        CreateMap<CreateCommentDto, CommentModel>();
        CreateMap<CommentModel, CommentDto>().ReverseMap();
    }
}