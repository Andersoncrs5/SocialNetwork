using AutoMapper;
using SocialNetwork.Contracts.DTOs.PostFavorite;
using SocialNetwork.Write.API.Models;

namespace SocialNetwork.Write.API.Utils.Mappers;

public class PostFavoriteMapper: Profile
{
    public PostFavoriteMapper()
    {
        CreateMap<PostFavoriteDto, PostFavoriteModel>().ReverseMap();
    }
}