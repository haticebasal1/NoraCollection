using System;
using NoraCollection.Shared.Dtos.AuthDtos;

namespace NoraCollection.Shared.Dtos.CartDtos;

public class CartDto
{
public int Id { get; set; }
public string? UserId { get; set; }
public UserDto? User { get; set; }

}
