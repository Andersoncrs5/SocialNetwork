using Microsoft.EntityFrameworkCore;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Configs.DB;
using SocialNetwork.Write.API.Models.Enums.Post;
using SocialNetwork.Write.API.Modules.PostReactions.Dto;
using SocialNetwork.Write.API.Modules.PostReactions.Model;
using SocialNetwork.Write.API.Modules.PostReactions.Service.Interface;
using SocialNetwork.Write.API.Utils.Classes;
using SocialNetwork.Write.API.Utils.result;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Write.API.Modules.PostReactions.Service.Provider;

public class PostReactionService(IUnitOfWork uow, ILogger<PostReactionService> logger): IPostReactionService
{
    public async Task<Result<PostReactionModel>> Create(TogglePostReactionDto dto, [IsId] string userId, bool commit = true)
    {
        try
        {
            PostReactionModel model = new PostReactionModel()
            {
                PostId = dto.PostId,
                ReactionId = dto.ReactionId,
                UserId = userId,
            };

            PostReactionModel async = await uow.PostReactionRepository.AddAsync(model);
            if (commit) await uow.CommitAsync();
        
            return Result<PostReactionModel>.Created(async);
        }
        catch (DbUpdateException ex) when (ex.IsDuplicateEntry())
        {
            return Result<PostReactionModel>.Conflict(
                "You have already reacted on this post."
            );
        }
        catch (DbUpdateException ex) when (ex.IsForeignKeyViolation())
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error creating post reaction. UserId={UserId}, PostId={PostId}",
                userId,
                dto.PostId
            );

            return Result<PostReactionModel>.Fail(
                "Error processing your reaction.",
                status: StatusCodes.Status500InternalServerError
            );
        }
    }
    
    public async Task DeleteAsync(PostReactionModel model, bool commit = true) 
    {
        uow.PostReactionRepository.Delete(model);
        if (commit) await uow.CommitAsync();
    }

    public async Task<Result<ResultToggle<PostReactionModel?>>> ToggleAsync(
        TogglePostReactionDto dto,
        [IsId] string userId,
        bool commit = true
    )
    {
        var reactionModel = await uow.PostReactionRepository
            .GetByUserIdAndPostId(userId, dto.PostId);

        // 🔴 REMOVE
        if (reactionModel != null && reactionModel.ReactionId == dto.ReactionId)
        {
            await DeleteAsync(reactionModel, false);
            
            if (commit)
                await uow.CommitAsync();

            return Result<ResultToggle<PostReactionModel?>>.Ok(
                new ResultToggle<PostReactionModel?>
                {
                    Action = ToggleStatus.Removed,
                    Value = null
                },
                message: "Reaction removed from post"
            );
        }

        // 🟡 UPDATE
        if (reactionModel != null)
        {
            reactionModel.ReactionId = dto.ReactionId;

            await uow.PostReactionRepository.Update(reactionModel);

            if (commit)
                await uow.CommitAsync();

            return Result<ResultToggle<PostReactionModel?>>.Ok(
                new ResultToggle<PostReactionModel?>
                {
                    Action = ToggleStatus.Update,
                    Value = reactionModel
                },
                message: "Reaction updated"
            );
        }

        // 🟢 CREATE
        var createResult = await Create(dto, userId, false);

        if (!createResult.Success)
            return Result<ResultToggle<PostReactionModel?>>.Fail(
                createResult.Message,
                createResult.Errors,
                createResult.Status
            );

        if (commit)
            await uow.CommitAsync();

        return Result<ResultToggle<PostReactionModel?>>.Created(
            new ResultToggle<PostReactionModel?>
            {
                Action = ToggleStatus.Added,
                Value = createResult.Value 
            },
            message: "Reaction added to post"
        );
    }
    
}