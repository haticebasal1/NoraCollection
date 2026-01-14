using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NoraCollection.Data.Abstract;
using NoraCollection.Entities.Abstract;

namespace NoraCollection.Data.Concrete.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity>
where TEntity : class, IEntity
{
    private readonly AppDbContext _appDbContext;
    public GenericRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await _appDbContext.Set<TEntity>().AddAsync(entity);
        return entity;
    }

    public void BulkDelete(IEnumerable<TEntity> entities)
    {
        _appDbContext.Set<TEntity>().RemoveRange(entities);
    }

    public void BulkUpdate(IEnumerable<TEntity> entities)
    {
        _appDbContext.Set<TEntity>().UpdateRange(entities);
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, bool includeDeleted = false, params Func<IQueryable<TEntity>, IQueryable<TEntity>>[] includes)
    {
        IQueryable<TEntity> query = _appDbContext.Set<TEntity>();
        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }
        if (includes?.Any() == true)
        {
            query = includes.Aggregate(query, (current, include) => include(current));
        }
        return await query.CountAsync();
    }

    public void Delete(TEntity entity)
    {
        _appDbContext.Set<TEntity>().Remove(entity);
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _appDbContext.Set<TEntity>().AnyAsync(predicate);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null, int? top = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderby = null, bool ? includeDeleted = false, params Func<IQueryable<TEntity>, IQueryable<TEntity>>[] includes)
    {
       IQueryable<TEntity> query = _appDbContext.Set<TEntity>();
       if (includeDeleted==true)
       {
        query = query.IgnoreQueryFilters();
       }
       if (predicate is not null)
       {
        query = query.Where(predicate);
       }
       if (orderby is not null)
       {
        query = orderby(query);
       }
       if (top is not null)
       {
        query = query.Take(top.Value);
       }
       if (includes?.Any() ==true)
       {
        query = includes.Aggregate(query,(current,include) => include(current));
       }
       return await query.ToListAsync();
    }

    public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, bool includeDeleted = false, params Func<IQueryable<TEntity>, IQueryable<TEntity>>[] includes)
    {
        IQueryable<TEntity> query = _appDbContext.Set<TEntity>();
        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }
        if (includes?.Any() == true)
        {
            query = includes.Aggregate(query,(current,include)=> include(current));
        }
        return await query.FirstOrDefaultAsync();
    }

    public void Update(TEntity entity)
    {
        _appDbContext.Set<TEntity>().Update(entity);
    }
}
