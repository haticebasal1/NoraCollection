using System;

namespace NoraCollection.Shared.Dtos.FavoriteDtos;

public class FavoriteDto
{
public int Id { get; set; }
public string? UserId { get; set; }
public int ProductId { get; set; }
public DateTime CreatedDate { get; set; }
public DateTime UpdatedDate { get; set; }
public string? ProductName { get; set; }
public decimal Price { get; set; }
public string? ImageUrl { get; set; }
public bool IsInStock { get; set; }
}
