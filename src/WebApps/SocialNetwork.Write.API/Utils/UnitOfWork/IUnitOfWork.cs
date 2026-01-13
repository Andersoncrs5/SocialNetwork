using Microsoft.EntityFrameworkCore.Storage;
using SocialNetwork.Write.API.Repositories.Interfaces;

namespace SocialNetwork.Write.API.Utils.UnitOfWork;

public interface IUnitOfWork: IDisposable
{
    IUserRepository UserRepository { get; }
    IRoleRepository RoleRepository { get; }
    
    Task CommitAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task ExecuteTransactionAsync(Func<Task> operation);
}