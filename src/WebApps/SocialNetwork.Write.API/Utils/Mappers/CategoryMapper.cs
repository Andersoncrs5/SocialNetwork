using AutoMapper;
using SocialNetwork.Contracts.DTOs.User;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Modules.Category.Dto;
using SocialNetwork.Write.API.Modules.Category.Model;

namespace SocialNetwork.Write.API.Utils.Mappers;

public class CategoryMapper: Profile
{
    public CategoryMapper()
    {
        CreateMap<CreateCategoryDto, CategoryModel>();
        CreateMap<CategoryModel, CategoryDto>().ReverseMap();
    }
}