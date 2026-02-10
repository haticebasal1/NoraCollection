using System;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.OrderDtos;

namespace NoraCollection.Data.Abstract;

public interface IOrderRepository : IGenericRepository<Order>
{
  // Ciro hesabı için asenkron metot
  Task<decimal> GetOrdersTotalAsync(OrderFiltersDto orderFiltersDto);

    // Yeni: sayfalı sipariş listesi
    Task<IEnumerable<Order>> GetPagedAsync(OrderFiltersDto filters);
}
