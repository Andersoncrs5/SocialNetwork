using AutoMapper;
using SocialNetwork.Contracts.DTOs.User;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Modules.User.Dto;
using SocialNetwork.Write.API.Modules.User.Model;

namespace SocialNetwork.Write.API.Utils.Mappers;

public class UserMapper: Profile
{
    public UserMapper()
    {
        CreateMap<CreateUserDto, UserModel>();
        CreateMap<UserModel, UserDto>().ReverseMap();
    }
}