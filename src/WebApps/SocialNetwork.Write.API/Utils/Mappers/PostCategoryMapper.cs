using AutoMapper;
using SocialNetwork.Contracts.DTOs.PostCategory;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Modules.PostCategory.Dto;
using SocialNetwork.Write.API.Modules.PostCategory.Model;

namespace SocialNetwork.Write.API.Utils.Mappers;

public class PostCategoryMapper: Profile
{
    public PostCategoryMapper()
    {
        CreateMap<PostCategoryModel, CreatePostCategoryDto>().ReverseMap();
        CreateMap<PostCategoryModel, PostCategoryDto>().ReverseMap();
        CreateMap<CreatePostCategoryDto, PostCategoryModel>()
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
            .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.PostId));
    }
}