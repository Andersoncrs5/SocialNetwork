using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.Modules.Reaction.Dto;
using SocialNetwork.Write.API.Modules.Reaction.Model;
using SocialNetwork.Write.API.Modules.Reaction.Service.Interface;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Write.API.Modules.Reaction.Service.Provider;

public class ReactionService(IUnitOfWork uow): IReactionService
{
    public async Task<ReactionModel> GetByIdAsync(string id)
        => await uow.ReactionRepository.GetByIdAsync(id) ?? throw new ModelNotFoundException("Reaction not found");

    public async Task<bool> ExistsByName(string name)
        => await uow.ReactionRepository.ExistsByName(name);

    public async Task<ReactionModel> CreateAsync(CreateReactionDto dto, bool commit = true)
    {
        ReactionModel reaction = uow.Mapper.Map<ReactionModel>(dto);

        ReactionModel reactionAdded = await uow.ReactionRepository.AddAsync(reaction);
        if (commit) await uow.CommitAsync();
        
        return reactionAdded;
    }

    public async Task DeleteAsync(ReactionModel reaction, bool commit = true)
    {
        await uow.ReactionRepository.DeleteAsync(reaction);
        if (commit) await uow.CommitAsync();
    }

    public async Task<ReactionModel> UpdateAsync(UpdateReactionDto dto, ReactionModel reaction, bool commit = true)
    {
        if (!string.IsNullOrWhiteSpace(dto.Name) && reaction.Name != dto.Name)
        {
            if (!await uow.ReactionRepository.ExistsByName(reaction.Name))
            {
                throw new ConflictValueException($"Name: {dto.Name} already in use"); 
            }
            
            reaction.Name = dto.Name;
        }
        
        if (dto.Type.HasValue && reaction.Type != dto.Type) reaction.Type = dto.Type.Value;
        if (!string.IsNullOrWhiteSpace(dto.EmojiUrl)) reaction.EmojiUrl = dto.EmojiUrl;
        if (!string.IsNullOrWhiteSpace(dto.EmojiUnicode)) reaction.EmojiUnicode = dto.EmojiUnicode;
        
        if (dto.DisplayOrder.HasValue && reaction.DisplayOrder != dto.DisplayOrder) reaction.DisplayOrder = dto.DisplayOrder.Value;
        if (dto.Active.HasValue && reaction.Active != dto.Active) reaction.Active = dto.Active.Value;
        if (dto.Visible.HasValue && reaction.Visible != dto.Visible) reaction.Visible = dto.Visible.Value;
        
        ReactionModel update = await uow.ReactionRepository.Update(reaction);
        if (commit) await uow.CommitAsync();
        
        return update;
    }
    
}