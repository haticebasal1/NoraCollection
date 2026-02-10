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

    public async Task<IEnumerable<Order>> GetPagedAsync(OrderFiltersDto filters)
    {
        var query = _appDbContext.Orders.AsNoTracking().AsQueryable();
        if (filters.OrderStatus.HasValue)
        {
            query = query.Where(o => o.OrderStatus == filters.OrderStatus.Value);
        }
        if (!string.IsNullOrEmpty(filters.UserId))
        {
            query = query.Where(o => o.UserId == filters.UserId);
        }
        if (filters.StartDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= filters.StartDate.Value);
        }
        if (filters.EndDate.HasValue)
        {
            var endOfDay = filters.EndDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(o => o.CreatedAt <= endOfDay);
        }
        if (filters.IsDeleted.HasValue)
        {
            query = query.Where(o => o.IsDeleted == filters.IsDeleted.Value);
        }
        else
        {
            query = query.Where(o => o.IsDeleted == false);
        }
        query = query.Include(o => o.User)
        .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
        .Include(o => o.OrderItems).ThenInclude(oi => oi.ProductVariant)
        .Include(o => o.OrderCoupons!).ThenInclude(oc => oc.Coupon);
        query = query.OrderByDescending(o => o.CreatedAt);
        var pageNumber = filters.PageNumber <= 0 ? 1 : filters.PageNumber;
        var pageSize = filters.PageSize <= 0 ? 20 : filters.PageSize;
        var skip = (pageNumber - 1) * pageSize;
        query = query.Skip(skip).Take(pageSize);
        return await query.ToListAsync();
    }
}
