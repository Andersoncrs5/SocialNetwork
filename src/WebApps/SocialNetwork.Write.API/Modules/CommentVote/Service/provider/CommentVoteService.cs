using Microsoft.EntityFrameworkCore;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Configs.DB;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.Models.Enums.Post;
using SocialNetwork.Write.API.Modules.CommentVote.Dto;
using SocialNetwork.Write.API.Modules.CommentVote.Model;
using SocialNetwork.Write.API.Modules.CommentVote.Service.Interface;
using SocialNetwork.Write.API.Utils.Classes;
using SocialNetwork.Write.API.Utils.Enums;
using SocialNetwork.Write.API.Utils.result;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Write.API.Modules.CommentVote.Service.provider;

public class CommentVoteService(
    IUnitOfWork uow,
    ILogger<CommentVoteService> logger
    ): ICommentVoteService
{
    public async Task<Result<CommentVoteModel>> Create(
        [IsId] string userId,
        [IsId] string commentId,
        VoteEnum vote,
        bool commit = true
    )
    {
        
        CommentVoteModel model = new CommentVoteModel()
        {
            CommentId = commentId, 
            UserId = userId, 
            Vote = vote
        };

        try
        {
            CommentVoteModel voteModel = await uow.CommentVoteRepository.AddAsync(model);
            if (commit) await uow.CommitAsync();
            
            return Result<CommentVoteModel>.Created(voteModel);
        }
        catch (DbUpdateException ex) when (ex.IsDuplicateEntry())
        {
            return Result<CommentVoteModel>.Conflict(
                "You have already voted on this post."
            );
        }
        catch (DbUpdateException ex) when (ex.IsForeignKeyViolation())
        {
            throw;
        }
        catch (DbUpdateException ex) when (ex.IsDataTooLong())
        {
            return Result<CommentVoteModel>.Fail(
                "One of the fields is too long.",
                status: StatusCodes.Status400BadRequest
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating post vote. UserId={UserId}, CommentId={CommentId}", userId, commentId);

            throw new InternalServerErrorException(ex.Message); 
        }
        
    }
    
    
    public async Task<Result> DeleteById([IsId] string id, bool commit = true)
    {
        try
        {
            bool deleted = await uow.CommentVoteRepository.DeleteById(id);

            if (!deleted) return Result.NotFound("Comment vote not found.");

            if (commit) await uow.CommitAsync();

            return Result.Ok("Comment vote deleted successfully.");
        }
        catch (DbUpdateException ex) when (ex.IsForeignKeyViolation())
        {
            return Result.Conflict("Cannot delete because it is referenced by other data.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error deleting post vote");
            
            return Result.Fail(
                "Error deleting post vote.",
                status: StatusCodes.Status500InternalServerError
            );
        }
    }

    
    private async Task<Result<CommentVoteModel>> UpdateVote(CommentVoteModel vote, VoteEnum voteEnum, bool commit = true)
    {
        vote.Vote = voteEnum;
        
        CommentVoteModel updated = await uow.CommentVoteRepository.Update(vote);
        
        if (commit) await uow.CommitAsync();
        
        return Result<CommentVoteModel>.Ok(updated);
    }

    
    public async Task<Result<ResultToggle<CommentVoteModel>>> Toggle(
        ToggleCommentVoteDto dto,
        [IsId] string userId,
        bool commit = true
        )
    {
        CommentVoteModel? model = await uow.CommentVoteRepository.GetByCommentIdAndUserId(
            commentId: dto.commentId,
            userId: userId
        );

        if (model == null)
        {
            Result<CommentVoteModel> createResult = await Create(userId, dto.commentId, dto.vote, false);

            if (!createResult.Success)
                return Result<ResultToggle<CommentVoteModel>>.Fail(
                    createResult.Message,
                    createResult.Errors,
                    createResult.Status
                );

            ResultToggle<CommentVoteModel> toggleResult = new()
            {
                Action = ToggleStatus.Added,
                Value = createResult.Value
            };

            if (commit) await uow.CommitAsync();

            return Result<ResultToggle<CommentVoteModel>>.Created(toggleResult, "Vote created");
        }

        if (!model.Vote.Equals(dto.vote))
        {
            model.Vote = dto.vote;

            Result<CommentVoteModel> updateResult = await UpdateVote(model, dto.vote, false);

            if (!updateResult.Success)
                return Result<ResultToggle<CommentVoteModel>>.Fail(
                    updateResult.Message,
                    updateResult.Errors,
                    updateResult.Status
                );

            ResultToggle<CommentVoteModel> toggleResult = new()
            {
                Action = ToggleStatus.Update,
                Value = updateResult.Value
            };

            if (commit)
                await uow.CommitAsync();

            return Result<ResultToggle<CommentVoteModel>>.Ok(toggleResult, "Vote updated");
        }

        Result deleteResult = await DeleteById(model.Id, false);

        if (!deleteResult.Success)
            return Result<ResultToggle<CommentVoteModel>>.Fail(
                deleteResult.Message,
                deleteResult.Errors,
                deleteResult.Status
            );

        ResultToggle<CommentVoteModel> removedResult = new()
        {
            Action = ToggleStatus.Removed,
            Value = null
        };

        if (commit) await uow.CommitAsync();

        return Result<ResultToggle<CommentVoteModel>>.Ok(removedResult, "Vote removed successfully.");
    }
    
    
}