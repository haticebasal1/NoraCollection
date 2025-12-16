using System;
using NoraCollection.Entities.Abstract;

namespace NoraCollection.Data.Abstract;

public interface IUnitOfWork:IDisposable
{
  int Save();
  Task<int> SaveAsync();
  IGenerıcRepository<TEntity> GetRepository<TEntity>() where TEntity : class,IEntity;
}
