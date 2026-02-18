using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Modules.Comment.Dto;
using SocialNetwork.Write.API.Modules.Comment.Model;
using SocialNetwork.Write.API.Modules.Comment.Service.Interface;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Write.API.Modules.Comment.Service.Provider;

public class CommentService(IUnitOfWork uow): ICommentService
{
    public async Task<CommentModel> GetByIdSimpleAsync([IsId] string id)
        => await uow.CommentRepository.GetByIdAsync(id) 
           ?? throw new ModelNotFoundException("Comment not found");

    public async Task DeleteAsync(CommentModel comment, bool commit = true)
    {
        await uow.CommentRepository.DeleteAsync(comment);
        if (commit) await uow.CommitAsync();
    }

    public async Task<bool> ExistsById([IsId] string id)
        => await uow.CommentRepository.ExistsById(id);

    public async Task<CommentModel> CreateAsync(CreateCommentDto dto, UserModel user, bool commit = true)
    {
        CommentModel map = uow.Mapper.Map<CommentModel>(dto);
        map.UserId = user.Id;

        CommentModel commentAdded = await uow.CommentRepository.AddAsync(map);
        if (commit) await uow.CommitAsync();
        
        return commentAdded;
    }

    public async Task<CommentModel> UpdateAsync(UpdateCommentDto dto, CommentModel comment, bool commit = true)
    {
        if (!string.IsNullOrWhiteSpace(dto.Content)) comment.Content = dto.Content;

        CommentModel commentUpdated = await uow.CommentRepository.Update(comment);
        if (commit) await uow.CommitAsync();
        return commentUpdated;
    }
    
}