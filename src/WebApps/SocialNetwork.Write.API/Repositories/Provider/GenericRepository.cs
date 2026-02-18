using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Configs.DB;
using SocialNetwork.Write.API.Repositories.Interfaces;
using SocialNetwork.Write.API.Services.Interfaces;
using SocialNetwork.Write.API.Utils.Bases;

namespace SocialNetwork.Write.API.Repositories.Provider;

public class GenericRepository<T>(AppDbContext context, IRedisService redisService) : IGenericRepository<T> where T : BaseModel
{
    protected readonly AppDbContext Context = context;
    private readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task<T?> GetByIdInCache([IsId] string id, TimeSpan? cacheExpiry = null)
    {
        T? inCache = await redisService.GetAsync<T>(id);
        
        if (inCache != null)
            return inCache;
        
        T? fromDb = await GetByIdAsync(id);
        
        if (fromDb != null)
            await redisService.CreateAsync(id, fromDb, cacheExpiry);
        
        return fromDb;
    }

    public async Task<bool> ExistsById([IsId] string Id)
    {
        return await _dbSet
            .AnyAsync(c => c.Id == Id);
    }
    
    public async Task<int> CountByIdAsync([IsId] string Id)
    {
        return await _dbSet
            .CountAsync(c => c.Id == Id);
    }
    
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await this._dbSet.ToListAsync();
    }
    
    public IQueryable<T> Query()
    {
        return this._dbSet;
    }

    public async Task<T?> GetByIdAsync([IsId] string id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<T> AddAsync(T entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        EntityEntry<T> data = await _dbSet.AddAsync(entity);
        return data.Entity;
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }
    
    public async Task<T> Update(T entity)
    {
        EntityEntry<T> update = _dbSet.Update(entity);
        return update.Entity;
    }

    public async Task UpdateRangeAsync(IEnumerable<T> entities)
    {
        await Task.Run(() => _dbSet.UpdateRange(entities));
    }
    
    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task DeleteAsync(T entity)
    {
        await Task.Run(() => _dbSet.Remove(entity));
    }

    public async Task DeleteRangeAsync(IEnumerable<T> entities)
    {
        await Task.Run(() => _dbSet.RemoveRange(entities));
    }
    
    public async Task SaveChangesAsync()
    {
        await Context.SaveChangesAsync();
    }
    
    public async Task<T?> GetSingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.SingleOrDefaultAsync(predicate);
    }
    
    public async Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        if (predicate == null)
            return await _dbSet.CountAsync();
            
        return await _dbSet.CountAsync(predicate);
    }
    
}