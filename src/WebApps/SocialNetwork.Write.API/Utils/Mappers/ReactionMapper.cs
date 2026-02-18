using AutoMapper;
using SocialNetwork.Contracts.DTOs.Reaction;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Modules.Reaction.Dto;
using SocialNetwork.Write.API.Modules.Reaction.Model;

namespace SocialNetwork.Write.API.Utils.Mappers;

public class ReactionMapper: Profile
{
    public ReactionMapper()
    {
        CreateMap<CreateReactionDto, ReactionModel>();
        CreateMap<ReactionModel, ReactionDto>().ReverseMap();
    }
}