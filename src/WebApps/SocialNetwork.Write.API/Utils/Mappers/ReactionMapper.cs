using AutoMapper;
using SocialNetwork.Write.API.dto.Reaction;
using SocialNetwork.Write.API.Models;

namespace SocialNetwork.Write.API.Utils.Mappers;

public class ReactionMapper: Profile
{
    public ReactionMapper()
    {
        CreateMap<CreateReactionDto, ReactionModel>();
        CreateMap<ReactionModel, ReactionDto>().ReverseMap();
    }
}