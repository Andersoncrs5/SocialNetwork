using Microsoft.EntityFrameworkCore;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Configs.DB;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.Models.Enums.Post;
using SocialNetwork.Write.API.Modules.PostVote.Dto;
using SocialNetwork.Write.API.Modules.PostVote.Gateway;
using SocialNetwork.Write.API.Modules.PostVote.Model;
using SocialNetwork.Write.API.Modules.PostVote.Service.Interface;
using SocialNetwork.Write.API.Utils.Classes;
using SocialNetwork.Write.API.Utils.Enums;
using SocialNetwork.Write.API.Utils.result;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Write.API.Modules.PostVote.Service.provider;

public class PostVoteService(IUnitOfWork uow, ILogger<PostVoteService> logger, PostVoteModuleGateway gateway): IPostVoteService
{
    public async Task<Result<PostVoteModel>> Create(
        [IsId] string userId, 
        [IsId] string postId, 
        VoteEnum vote, 
        bool commit = true
        )
    {
        bool existsPost = await gateway.ExistsPostById(postId);
        if (!existsPost) return Result<PostVoteModel>.NotFound("Post not found");
        
        if (string.IsNullOrWhiteSpace(userId))
            return Result<PostVoteModel>.Fail("UserId is required.", status: StatusCodes.Status400BadRequest);

        if (string.IsNullOrWhiteSpace(postId))
            return Result<PostVoteModel>.Fail("PostId is required.", status: StatusCodes.Status400BadRequest);
        
        PostVoteModel model = new PostVoteModel()
        {
            PostId = postId, 
            UserId = userId, 
            Vote = vote
        };

        try
        {
            PostVoteModel voteModel = await uow.PostVoteRepository.AddAsync(model);
            if (commit) await uow.CommitAsync();
            
            return Result<PostVoteModel>.Created(voteModel);
        }
        catch (DbUpdateException ex) when (ex.IsDuplicateEntry())
        {
            return Result<PostVoteModel>.Conflict(
                "You have already voted on this post."
            );
        }
        catch (DbUpdateException ex) when (ex.IsForeignKeyViolation())
        {
            Exception? current = ex;
            while (current?.InnerException != null)
                current = current.InnerException;

            var message = current?.Message ?? ex.Message;

            if (message.Contains("FK_PostVotes_Posts_PostId", StringComparison.OrdinalIgnoreCase))
                return Result<PostVoteModel>.NotFound("Post not found.");

            if (message.Contains("FK_PostVotes_Users_UserId", StringComparison.OrdinalIgnoreCase))
                return Result<PostVoteModel>.NotFound("User not found.");

            return Result<PostVoteModel>.NotFound("User or post not found.");
        }
        catch (DbUpdateException ex) when (ex.IsDataTooLong())
        {
            return Result<PostVoteModel>.Fail(
                "One of the fields is too long.",
                status: StatusCodes.Status400BadRequest
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating post vote. UserId={UserId}, PostId={PostId}", userId, postId);

            return Result<PostVoteModel>.Fail(
                "Error processing your vote.",
                status: StatusCodes.Status500InternalServerError
            );
        }
    }

    public async Task<Result> DeleteById([IsId] string id, bool commit = true)
    {
        try
        {
            bool deleted = await uow.PostVoteRepository.DeleteById(id);

            if (!deleted) return Result.NotFound("Post vote not found.");

            if (commit) await uow.CommitAsync();

            return Result.Ok("Post vote deleted successfully.");
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

    private async Task<Result<PostVoteModel>> UpdateVote(PostVoteModel vote, VoteEnum voteEnum, bool commit = true)
    {
        vote.Vote = voteEnum;
        
        PostVoteModel updated = await uow.PostVoteRepository.Update(vote);
        
        if (commit) await uow.CommitAsync();
        
        return Result<PostVoteModel>.Ok(updated);
    }

    public async Task<Result<ResultToggle<PostVoteModel>>> Toggle(
        TogglePostVoteDto dto,
        [IsId] string userId,
        bool commit = true
        )
    {
        PostVoteModel? model = await uow.PostVoteRepository.GetByPostIdAndUserId(
            postId: dto.postId,
            userId: userId
        );

        if (model == null)
        {
            Result<PostVoteModel> createResult = await Create(userId, dto.postId, dto.vote, false);

            if (!createResult.Success)
                return Result<ResultToggle<PostVoteModel>>.Fail(
                    createResult.Message,
                    createResult.Errors,
                    createResult.Status
                );

            ResultToggle<PostVoteModel> toggleResult = new()
            {
                Action = ToggleStatus.Added,
                Value = createResult.Value
            };

            if (commit) await uow.CommitAsync();

            return Result<ResultToggle<PostVoteModel>>.Created(toggleResult);
        }

        if (!model.Vote.Equals(dto.vote))
        {
            model.Vote = dto.vote;

            Result<PostVoteModel> updateResult = await UpdateVote(model, dto.vote, false);

            if (!updateResult.Success)
                return Result<ResultToggle<PostVoteModel>>.Fail(
                    updateResult.Message,
                    updateResult.Errors,
                    updateResult.Status
                );

            ResultToggle<PostVoteModel> toggleResult = new()
            {
                Action = ToggleStatus.Update,
                Value = updateResult.Value
            };

            if (commit)
                await uow.CommitAsync();

            return Result<ResultToggle<PostVoteModel>>.Ok(toggleResult);
        }

        Result deleteResult = await DeleteById(model.Id, false);

        if (!deleteResult.Success)
            return Result<ResultToggle<PostVoteModel>>.Fail(
                deleteResult.Message,
                deleteResult.Errors,
                deleteResult.Status
            );

        ResultToggle<PostVoteModel> removedResult = new()
        {
            Action = ToggleStatus.Removed,
            Value = null
        };

        if (commit) await uow.CommitAsync();

        return Result<ResultToggle<PostVoteModel>>.Ok(removedResult);
    }
    
}