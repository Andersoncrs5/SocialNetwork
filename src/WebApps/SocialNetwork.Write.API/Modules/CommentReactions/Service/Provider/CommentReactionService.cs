using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Configs.DB;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.Models.Enums.Post;
using SocialNetwork.Write.API.Modules.CommentReactions.Dto;
using SocialNetwork.Write.API.Modules.CommentReactions.Model;
using SocialNetwork.Write.API.Modules.CommentReactions.Service.Interface;
using SocialNetwork.Write.API.Utils.Classes;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Write.API.Modules.CommentReactions.Service.Provider;

public class CommentReactionService(IUnitOfWork uow): ICommentReactionService
{
    public async Task<CommentReactionModel> GetByIdSimpleAsync([IsId] string id)
        => await uow.CommentReactionRepository.GetByIdAsync(id) ?? throw new ModelNotFoundException("Comment reaction not found");

    public async Task DeleteAsync(CommentReactionModel model, bool commit = true)
    {
        await uow.CommentReactionRepository.DeleteAsync(model);
        if (commit) await uow.CommitAsync();
    }

    public async Task<CommentReactionModel> CreateAsync(ToggleCommentReactionDto dto, [IsId] string userId, bool commit = true)
    {
        try
        {
            CommentReactionModel model = uow.Mapper.Map<CommentReactionModel>(dto);
            model.UserId = userId;

            CommentReactionModel added = await uow.CommentReactionRepository.AddAsync(model);
            if (commit) await uow.CommitAsync();
        
            return added;
        }
        catch (Exception ex) when (ex.IsDuplicateEntry())
        {
            throw new ConflictValueException("You've already reacted to this comment!");
        }
    }

    public async Task<ResultToggle<CommentReactionModel?>> ToggleAsync(ToggleCommentReactionDto dto, [IsId] string userId, bool commit = true)
    {
        var reactionModel = await uow.CommentReactionRepository.GetByUserIdAndCommentId(userId, dto.CommentId);

        if (reactionModel != null && reactionModel.ReactionId == dto.ReactionId)
        {
            await uow.CommentReactionRepository.DeleteAsync(reactionModel);
            if (commit) await uow.CommitAsync();
        
            return new ResultToggle<CommentReactionModel?> { Action = ToggleStatus.Removed, Value = null };
        }

        if (reactionModel != null)
        {
            reactionModel.ReactionId = dto.ReactionId;

        
            await uow.CommentReactionRepository.Update(reactionModel);
            if (commit) await uow.CommitAsync();
    
            return new ResultToggle<CommentReactionModel?> { Action = ToggleStatus.Update, Value = reactionModel };
        }

        try 
        {
            var map = uow.Mapper.Map<CommentReactionModel>(dto);
            map.UserId = userId;

            await uow.CommentReactionRepository.AddAsync(map); 
            if (commit) await uow.CommitAsync();

            return new ResultToggle<CommentReactionModel?> { Action = ToggleStatus.Added, Value = map };
        }
        catch (Exception ex) when (ex.IsDuplicateEntry())
        {
            return new ResultToggle<CommentReactionModel?> { Action = ToggleStatus.Added, Value = null };
        }
    }
    
}