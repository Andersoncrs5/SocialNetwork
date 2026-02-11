using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.dto.Comment;
using SocialNetwork.Write.API.Models;

namespace SocialNetwork.Write.API.Services.Interfaces;

public interface ICommentService
{
    Task<bool> ExistsById([IsId] string id);
    Task<CommentModel> GetByIdSimpleAsync([IsId] string id);
    Task DeleteAsync(CommentModel comment, bool commit = true);
    Task<CommentModel> CreateAsync(CreateCommentDto dto, UserModel user, bool commit = true);
    Task<CommentModel> UpdateAsync(UpdateCommentDto dto, CommentModel comment, bool commit = true);
}