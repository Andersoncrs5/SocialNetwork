using AutoMapper;
using SocialNetwork.Contracts.DTOs.CommentFavorite;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Modules.CommentFavorite.Model;

namespace SocialNetwork.Write.API.Utils.Mappers;

public class CommentFavoriteMapper: Profile
{
    public CommentFavoriteMapper()
    {
        CreateMap<CommentFavoriteDto, CommentFavoriteModel>().ReverseMap();
    }
}