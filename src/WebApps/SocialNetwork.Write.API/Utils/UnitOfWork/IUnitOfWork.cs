using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using SocialNetwork.Write.API.Modules.Category.Repository.Interface;
using SocialNetwork.Write.API.Modules.Comment.Repository.Interface;
using SocialNetwork.Write.API.Modules.Post.Repository.Interface;
using SocialNetwork.Write.API.Repositories.Interfaces;
using SocialNetwork.Write.API.Services.Interfaces;

namespace SocialNetwork.Write.API.Utils.UnitOfWork;

public interface IUnitOfWork: IDisposable
{
    IUserRepository UserRepository { get; }
    IRoleRepository RoleRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    ITagRepository TagRepository { get; }
    IPostRepository PostRepository { get; }
    IRedisService  RedisService { get; }
    IPostCategoryRepository PostCategoryRepository { get; }
    IPostTagRepository PostTagRepository { get; }
    ICommentRepository CommentRepository { get; }
    IPostFavoriteRepository PostFavoriteRepository { get;  }
    ICommentFavoriteRepository CommentFavoriteRepository { get;  }
    IReactionRepository ReactionRepository { get; }
    IMapper Mapper { get; }
    
    Task CommitAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task ExecuteTransactionAsync(Func<Task> operation);
}