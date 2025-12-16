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
}
