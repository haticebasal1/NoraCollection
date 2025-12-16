using System;
using System.Linq.Expressions;
using NoraCollection.Entities.Abstract;

namespace NoraCollection.Data.Abstract;

public interface IGenericRepository<TEntity> where TEntity : class,IEntity
{
  Task<TEntity> GetAsync(
    Expression<Func<TEntity, bool>>
    predicate,
    bool includeDeleted = false,
    params Func<IQueryable<TEntity>,
    IQueryable<TEntity>>[] includes
  );
  Task<IEnumerable<TEntity>> GetAllAsync(
     Expression<Func<TEntity,bool>>?
     predicate = null,
     int? top = null,
     Func<IQueryable<TEntity>,
     IOrderedQueryable<TEntity>>? orderby = null,
     bool ? includeDeleted = false,
     params Func<IQueryable<TEntity>,
     IQueryable<TEntity>>[] includes
  );
  Task<bool> ExistsAsync(
    Expression<Func<TEntity, bool>>predicate);
 Task<int> CountAsync(
    Expression<Func<TEntity, bool>>?
    predicate = null,
    bool includeDeleted = false,
    params Func<IQueryable<TEntity>,
    IQueryable<TEntity>>[] includes
 );
 Task<TEntity> AddAsync(TEntity entity);
 void Update (TEntity entity);
 void BulkUpdate(IEnumerable<TEntity> entities);
 void BulkDelete(IEnumerable<TEntity>entities);
 void Delete(TEntity entity);
}
