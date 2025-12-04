using System;
using NoraCollection.Shared.Dtos.CategoryDtos;

namespace NoraCollection.Shared.Dtos.ProductDtos;

public class ProductDto
{
  public int Id { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
  public DateTime DeletedAt { get; set; }
  public bool IsDeleted { get; set; }
  public string? Name { get; set; }
  public string? Properties { get; set; }
  public string? Price { get; set; }
  public string? ImageUrl { get; set; }
  public bool  IsHome { get; set; }
  public int Stock { get; set; }
  public bool IsInStock => Stock>0;
  public ICollection<CategoryDto>? Categories { get; set; }
}
