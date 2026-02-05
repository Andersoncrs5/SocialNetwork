using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Distributed;
using SocialNetwork.Write.API.Configs.DB;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Repositories.Interfaces;
using SocialNetwork.Write.API.Repositories.Provider;
using SocialNetwork.Write.API.Services.Interfaces;

namespace SocialNetwork.Write.API.Utils.UnitOfWork;

public class UnitOfWork(
    AppDbContext context, 
    UserManager<UserModel> userManager, 
    RoleManager<RoleModel> roleManager,
    IDistributedCache db,
    IRedisService redisService,
    IMapper mapper
) : IUnitOfWork, IDisposable
{
    private UserRepository? _userRepository;
    private RoleRepository? _roleRepository;
    private CategoryRepository? _categoryRepository;
    private TagRepository? _tagRepository;
    private PostRepository? _postRepository;
    
    public IUserRepository UserRepository
        => _userRepository ??= new UserRepository(context, userManager);
    public IRoleRepository RoleRepository
        => _roleRepository ??= new RoleRepository(roleManager);
    public ICategoryRepository CategoryRepository
        => _categoryRepository ??= new CategoryRepository(context, redisService);
    public ITagRepository TagRepository
        => _tagRepository ??= new TagRepository(context, redisService); 
    public IPostRepository PostRepository
        => _postRepository ??= new PostRepository(context, redisService);
    
    public async Task CommitAsync() 
    {
        try
        {
            await context.SaveChangesAsync();
        } 
        catch (DbUpdateException ex)
        {
            var innerEx = ex.InnerException; 
        
            if (innerEx != null)
            {
                Console.WriteLine($"SQL COMMIT ERROR: {innerEx.Message}");
                throw new InvalidOperationException($"Database Integrity Violation: {innerEx.Message}", innerEx);
            }

            throw; 
        }
    }
    
    public Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return context.Database.BeginTransactionAsync();
    }

    public void Dispose()
    {
        context.Dispose();
        GC.SuppressFinalize(this);
    }
    
    public async Task ExecuteTransactionAsync(Func<Task> operation)
    {
        IDbContextTransaction? transaction = null;

        try
        {
            transaction = await BeginTransactionAsync();

            await operation.Invoke();

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            if (transaction != null)
            {
                await transaction.RollbackAsync();
            }
            throw; 
        }
        finally
        {
            if (transaction != null)
            {
                await transaction.DisposeAsync();
            }
        }
    }
}