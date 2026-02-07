using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NoraCollection.Data.Abstract;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.OrderDtos;

namespace NoraCollection.Data.Concrete.Repositories;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    private readonly AppDbContext _appDbContext;

    public OrderRepository(AppDbContext appDbContext) : base(appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public Task<decimal> GetOrdersTotalAsync(OrderFiltersDto orderFiltersDto)
    {
        var query = _appDbContext.Orders.AsNoTracking().AsQueryable();
        if (orderFiltersDto.OrderStatus.HasValue)
        {
            query = query.Where(o => o.OrderStatus == orderFiltersDto.OrderStatus.Value);
        }
        if (!string.IsNullOrEmpty(orderFiltersDto.UserId))
        {
            query = query.Where(o => o.UserId == orderFiltersDto.UserId);
        }
        if (orderFiltersDto.StartDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= orderFiltersDto.StartDate.Value);
        }
        if (orderFiltersDto.EndDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= orderFiltersDto.EndDate.Value);
        }
        if (orderFiltersDto.IsDeleted.HasValue)
        {
            query = query.Where(o => o.IsDeleted == orderFiltersDto.IsDeleted.Value);
        }
        else
        {
            query = query.Where(o => o.IsDeleted == false);
        }
        var total = query.SumAsync(o => o.FinalTotal);
        return total;
    }
}
