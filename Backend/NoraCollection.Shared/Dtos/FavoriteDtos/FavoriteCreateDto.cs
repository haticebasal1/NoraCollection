using System;

namespace NoraCollection.Shared.Dtos.FavoriteDtos;

public class FavoriteCreateDto
{
 public string? UserId { get; set; }
 public int ProductId { get; set; }
 public DateTime CreatedDate { get; set; }= DateTime.UtcNow;
}
