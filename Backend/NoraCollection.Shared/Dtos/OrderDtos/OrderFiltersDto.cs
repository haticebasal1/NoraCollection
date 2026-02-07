using System;
using NoraCollection.Shared.Enums;

namespace NoraCollection.Shared.Dtos.OrderDtos;

public class OrderFiltersDto
{
    public OrderFiltersDto()
    {

    }

    public OrderFiltersDto(OrderStatus? orderStatus, string? userId, DateTime? startDate, DateTime? endDate, bool? isDeleted)
    {
        OrderStatus = orderStatus;
        UserId = userId;
        StartDate = startDate;
        EndDate = endDate;
        IsDeleted = isDeleted;
    }

    public OrderStatus? OrderStatus { get; set; } = null;
    public string? UserId { get; set; } = null;
    public DateTime? StartDate { get; set; } = null;
    public DateTime? EndDate { get; set; } = null;
    public bool? IsDeleted { get; set; } = null;
    
    // Sayfalama
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    // Finansal Filtreler
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    // Arama (Sipariş No, Müşteri Adı, Telefon vb. için)
    public string? SearchTerm { get; set; }
}
