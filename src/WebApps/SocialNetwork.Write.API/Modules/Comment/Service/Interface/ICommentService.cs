using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Modules.Comment.Dto;
using SocialNetwork.Write.API.Modules.Comment.Model;

namespace SocialNetwork.Write.API.Modules.Comment.Service.Interface;

public interface ICommentService
{
    Task<bool> ExistsById([IsId] string id);
    Task<CommentModel> GetByIdSimpleAsync([IsId] string id);
    Task DeleteAsync(CommentModel comment, bool commit = true);
    Task<CommentModel> CreateAsync(CreateCommentDto dto, UserModel user, bool commit = true);
    Task<CommentModel> UpdateAsync(UpdateCommentDto dto, CommentModel comment, bool commit = true);
}