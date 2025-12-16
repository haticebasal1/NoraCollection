using System;
using Microsoft.Extensions.DependencyInjection;
using NoraCollection.Data.Abstract;

namespace NoraCollection.Data.Concrete;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _appDbContext;
    private readonly IServiceProvider _serviceProvider;

    public UnitOfWork(AppDbContext appDbContext, IServiceProvider serviceProvider)
    {
        _appDbContext = appDbContext;
        _serviceProvider = serviceProvider;
    }

    public void Dispose()
    {
        _appDbContext.Dispose();
    }

    public int Save()
    {
        return _appDbContext.SaveChanges();
    }

    public async Task<int> SaveAsync()
    {
        return await _appDbContext.SaveChangesAsync();
    }

    IGenerıcRepository<TEntity> IUnitOfWork.GetRepository<TEntity>()
    {
       var repository = _serviceProvider.GetRequiredService<IGenerıcRepository<TEntity>>();
       return repository;
    }
}
