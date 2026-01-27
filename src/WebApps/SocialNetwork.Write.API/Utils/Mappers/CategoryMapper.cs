using AutoMapper;
using SocialNetwork.Contracts.DTOs.User;
using SocialNetwork.Write.API.dto.Category;
using SocialNetwork.Write.API.Models;

namespace SocialNetwork.Write.API.Utils.Mappers;

public class CategoryMapper: Profile
{
    public CategoryMapper()
    {
        CreateMap<CreateCategoryDto, CategoryModel>();
        CreateMap<CategoryModel, CategoryDto>().ReverseMap();
    }
}