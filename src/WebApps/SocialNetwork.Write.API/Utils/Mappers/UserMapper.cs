using AutoMapper;
using SocialNetwork.Contracts.DTOs.User;
using SocialNetwork.Write.API.dto.User;
using SocialNetwork.Write.API.Models;

namespace SocialNetwork.Write.API.Utils.Mappers;

public class UserMapper: Profile
{
    public UserMapper()
    {
        CreateMap<CreateUserDto, UserModel>();
        CreateMap<UserModel, UserDto>();
    }
}