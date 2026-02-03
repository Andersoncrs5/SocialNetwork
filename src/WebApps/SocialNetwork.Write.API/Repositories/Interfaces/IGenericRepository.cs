using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SocialNetwork.Contracts.Attributes.Globals;

namespace SocialNetwork.Write.API.Repositories.Interfaces;

public interface IGenericRepository<T> where T: class
{
    Task<T?> GetByIdInCache([IsId] string id, TimeSpan? cacheExpiry = null);
    Task<IEnumerable<T>> GetAllAsync();
    Task<bool> ExistsById([IsId] string Id);
    Task<int> CountByIdAsync([IsId] string Id);
    IQueryable<T> Query();
    Task<T?> GetByIdAsync([IsId] string id);
    Task<T> AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    Task<T> Update(T entity);
    Task UpdateRangeAsync(IEnumerable<T> entities);
    void Delete(T entity);
    Task DeleteAsync(T entity);
    Task DeleteRangeAsync(IEnumerable<T> entities);
    Task SaveChangesAsync();
    Task<T?> GetSingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    
}