using SocialNetwork.Write.API.dto.Reaction;
using SocialNetwork.Write.API.Models;

namespace SocialNetwork.Write.API.Services.Interfaces;

public interface IReactionService
{
    Task<ReactionModel> GetByIdAsync(string id);
    Task<bool> ExistsByName(string name);
    Task<ReactionModel> CreateAsync(CreateReactionDto dto, bool commit = true);
    Task DeleteAsync(ReactionModel reaction, bool commit = true);
    Task<ReactionModel> UpdateAsync(UpdateReactionDto dto, ReactionModel reaction, bool commit = true);
}