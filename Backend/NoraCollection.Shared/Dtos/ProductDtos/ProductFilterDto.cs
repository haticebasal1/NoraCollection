using System;

namespace NoraCollection.Shared.Dtos.ProductDtos;

public class ProductFilterDto
{
    public bool IncludeCategories { get; set; } = false;
    public int? CategoryId { get; set; }
    public int? StoneTypeId { get; set; }
    public int? ColorId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? OrderBy { get; set; }
}